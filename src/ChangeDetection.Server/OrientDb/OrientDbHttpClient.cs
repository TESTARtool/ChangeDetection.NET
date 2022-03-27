using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Testar.ChangeDetection.Core;

namespace Testar.ChangeDetection.Server.OrientDb;

public class OrientDbHttpClient
{
    private readonly HttpClient client;
    private readonly IOptionsMonitor<OrientDbOptions> orientDbOptions;
    private readonly JsonSerializerOptions jsonOptions;
    private string? dbName;

    public OrientDbHttpClient(IHttpClientFactory httpClientFactory, IOptionsMonitor<OrientDbOptions> orientDbOptions)
    {
        this.client = httpClientFactory.CreateClient();
        this.orientDbOptions = orientDbOptions;
        this.client.BaseAddress = orientDbOptions.CurrentValue.OrientDbServerUrl;

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

        this.jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
    }

    public OrientDbHttpClient WithSession(IEnumerable<Claim> claims)
    {
        var orientDbSession = claims.First(x => x.Type == "OrientDbSession").Value;
        client.DefaultRequestHeaders.Add("Cookie", orientDbSession);

        if (orientDbOptions.CurrentValue.MultiDatabaseSupport)
        {
            dbName = claims.FirstOrDefault(x => x.Type == "DatabaseName")?.Value;
        }

        if (string.IsNullOrWhiteSpace(dbName))
        {
            dbName = orientDbOptions.CurrentValue.StateDatabaseName;
        }

        return this;
    }

    public OrientDbHttpClient WithUsernameAndPassword(string username, string password)
    {
        var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);

        return this;
    }

    public async Task<OrientDbSession?> LoginAsync(string? databaseName)
    {
        var dbName = databaseName ?? orientDbOptions.CurrentValue.StateDatabaseName;
        var response = await client.GetAsync($"connect/{dbName}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var sessionId = OrientDbSession.ParseSessionId(response.Headers);

        return new OrientDbSession(orientDbOptions.CurrentValue.OrientDbServerUrl, sessionId, dbName!);
    }

    public async Task<OrientDbResult> QueryAsync(OrientDbCommand orientDbCommand)
    {
        var url = $"/command/{dbName}/sql";

        var json = JsonSerializer.Serialize(orientDbCommand);
        using var httpContent = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json),
        };

        var response = await client.SendAsync(httpContent);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();

        var orientDbResult = await JsonSerializer.DeserializeAsync<OrientDbResult>(stream, jsonOptions)
            ?? throw new Exception("Unable to parse query result to JsonElement");

        return orientDbResult;
    }

    public async Task<OrientDbDocumentResult> DocumentAsync(OrientDbId orientDbId)
    {
        var url = $"/document/{dbName}/{orientDbId.FormatId()}";

        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();

        var orientDbResult = await JsonSerializer.DeserializeAsync<OrientDbDocumentResult>(stream, jsonOptions)
            ?? throw new Exception("Unable to parse query result to JsonElement");

        return orientDbResult;
    }
}