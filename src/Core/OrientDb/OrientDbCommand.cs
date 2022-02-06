using System.Text.Json;
using System.Text.Json.Serialization;

namespace Testar.ChangeDetection.Core.OrientDb;

public interface IOrientDbCommand
{
    Task<TElement[]> ExecuteQueryAsync<TElement>(string sql);

    Task<byte[]> ExecuteDocumentAsync(OrientDbId id);
}

public class OrientDbCommand : IOrientDbCommand
{
    private readonly IHttpClientFactory clientFactory;
    private readonly ILogger<OrientDbId> logger;
    private readonly IOrientDbSessionProvider orientDbSessionProvider;
    private readonly JsonSerializerOptions jsonOptions;

    public OrientDbCommand(IHttpClientFactory clientFactory, ILogger<OrientDbId> logger, IOrientDbSessionProvider orientDbSessionProvider)
    {
        this.clientFactory = clientFactory;
        this.logger = logger;
        this.orientDbSessionProvider = orientDbSessionProvider;
        this.jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
    }

    public async Task<TElement[]> ExecuteQueryAsync<TElement>(string sql)
    {
        var session = await orientDbSessionProvider.GetSessionAsync();
        var client = clientFactory.CreateOrientDbHttpClient(session.OrientDbUrl);
        client.DefaultRequestHeaders.Add("Cookie", session.SessionId);

        var url = $"query/{session.DatabaseName}/sql/{sql}";

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
        var session = await orientDbSessionProvider.GetSessionAsync();

        var client = clientFactory.CreateOrientDbHttpClient(session.OrientDbUrl);
        client.DefaultRequestHeaders.Add("Cookie", session.SessionId);

        var url = $"document/{session.DatabaseName}/{id.Value.Replace("#", "").Trim()}";

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