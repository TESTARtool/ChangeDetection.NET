using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Testar.ChangeDetection.Core;

namespace Testar.ChangeDetection.BlazorApp.OrientDb;

public interface IOrientDbCommand
{
    Task<TElement[]> ExecuteQueryAsync<TElement>(string sql, OrientDbOptions orientDbOptions);

    Task<byte[]> ExecuteDocumentAsync(OrientDbId id, OrientDbOptions orientDbOptions);
}

public class OrientDbCommand : IOrientDbCommand
{
    private readonly IHttpClientFactory clientFactory;
    private readonly ILogger<OrientDbId> logger;
    private readonly JsonSerializerOptions jsonSerializerOptions;

    public OrientDbCommand(IHttpClientFactory clientFactory, ILogger<OrientDbId> logger)
    {
        this.clientFactory = clientFactory;
        this.logger = logger;

        jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
    }

    public async Task<TElement[]> ExecuteQueryAsync<TElement>(string sql, OrientDbOptions orientDbOptions)
    {
        var client = CreateQueryClient(orientDbOptions);

        var response = await client.GetAsync(sql);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();

        var orientDbResult = (await JsonSerializer.DeserializeAsync<OrientDbResult>(stream, jsonSerializerOptions))
            ?? throw new Exception("Unable to parse query result to JsonElement");

        logger.LogExecutionPlan(sql, orientDbResult.ExecutionPlan);

        return orientDbResult.Result.Deserialize<TElement[]>(jsonSerializerOptions) ?? Array.Empty<TElement>();
    }

    public async Task<byte[]> ExecuteDocumentAsync(OrientDbId id, OrientDbOptions orientDbOptions)
    {
        var client = CreateDocumentClient(orientDbOptions);

        var response = await client.GetAsync(id.Value.Replace("#", "").Trim());
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();

        var orientDbResult = (await JsonSerializer.DeserializeAsync<OrientDbDocumentResult>(stream))
            ?? throw new Exception("Unable to parse query result to JsonElement");

        return string.IsNullOrWhiteSpace(orientDbResult.Value)
            ? Array.Empty<byte>()
            : Convert.FromBase64String(orientDbResult.Value);
    }

    private HttpClient CreateQueryClient(OrientDbOptions orientDbOptions) => CreateOrientDbClient($"query/{orientDbOptions.DatabaseName}/sql/", orientDbOptions);

    private HttpClient CreateDocumentClient(OrientDbOptions orientDbOptions) => CreateOrientDbClient($"document/{orientDbOptions.DatabaseName}/", orientDbOptions);

    private HttpClient CreateOrientDbClient(string uri, OrientDbOptions orientDbOptions)
    {
        var orientDbUrl = new Uri(orientDbOptions.Url, uri);

        var base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes($"{orientDbOptions.UserName}:{orientDbOptions.Password}"));
        var client = clientFactory.CreateClient();

        client.BaseAddress = orientDbUrl;
        client.DefaultRequestHeaders.Clear();

        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);

        return client;
    }

    private class OrientDbResult
    {
        public JsonElement ExecutionPlan { get; set; }

        public JsonElement Result { get; set; }
    }

    private class OrientDbDocumentResult
    {
        public string Value { get; set; }
    }
}