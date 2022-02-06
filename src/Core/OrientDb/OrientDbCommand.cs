using System.Text.Json;
using System.Text.Json.Serialization;

namespace Testar.ChangeDetection.Core.OrientDb;

public interface IOrientDbCommand
{
    Task<TElement[]> ExecuteQueryAsync<TElement>(string sql);

    Task<byte[]> ExecuteDocumentAsync(OrientDbId id);
}

public interface IOrientDbSignInProvider
{
    Task EnhanceHttpClient(HttpClient httpClient);

    Task<string?> GetDatabaseNameAsync();
}

public class OrientDbCommand : IOrientDbCommand
{
    private readonly IHttpClientFactory clientFactory;
    private readonly ILogger<OrientDbId> logger;
    private readonly IOrientDbSignInProvider orientDbSignInProvider;
    private readonly JsonSerializerOptions jsonOptions;

    public OrientDbCommand(IHttpClientFactory clientFactory, ILogger<OrientDbId> logger, IOrientDbSignInProvider orientDbSignInProvider)
    {
        this.clientFactory = clientFactory;
        this.logger = logger;
        this.orientDbSignInProvider = orientDbSignInProvider;
        this.jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
    }

    public async Task<TElement[]> ExecuteQueryAsync<TElement>(string sql)
    {
        var databaseName = await orientDbSignInProvider.GetDatabaseNameAsync();
        var client = clientFactory.CreateOrientDbHttpClient();

        await orientDbSignInProvider.EnhanceHttpClient(client);

        var url = $"query/{databaseName}/sql/{sql}";

        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();

        var orientDbResult = await JsonSerializer.DeserializeAsync<OrientDbResult>(stream)
            ?? throw new Exception("Unable to parse query result to JsonElement");

        logger.LogExecutionPlan(sql, orientDbResult.ExecutionPlan);

        return orientDbResult.Result.Deserialize<TElement[]>(jsonOptions) ?? Array.Empty<TElement>();
    }

    public async Task<byte[]> ExecuteDocumentAsync(OrientDbId id)
    {
        var databaseName = await orientDbSignInProvider.GetDatabaseNameAsync();

        var client = clientFactory.CreateOrientDbHttpClient();

        await orientDbSignInProvider.EnhanceHttpClient(client);

        var url = $"document/{databaseName}/{id.Value.Replace("#", "").Trim()}";

        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();

        var orientDbResult = await JsonSerializer.DeserializeAsync<OrientDbDocumentResult>(stream, jsonOptions)
            ?? throw new Exception("Unable to parse query result to JsonElement");

        return string.IsNullOrWhiteSpace(orientDbResult.Value)
            ? Array.Empty<byte>()
            : Convert.FromBase64String(orientDbResult.Value);
    }

    private class OrientDbResult
    {
        [JsonPropertyName("executionPlan")]
        public JsonElement ExecutionPlan { get; set; }

        [JsonPropertyName("result")]
        public JsonElement Result { get; set; }
    }

    private class OrientDbDocumentResult
    {
        public string Value { get; set; }
    }
}