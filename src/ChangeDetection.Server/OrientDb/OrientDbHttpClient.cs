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
    private readonly IDatabaseSelector databaseSelector;
    private readonly JsonSerializerOptions jsonOptions;

    public OrientDbHttpClient(IHttpClientFactory httpClientFactory, IOptionsMonitor<OrientDbOptions> orientDbOptions, IDatabaseSelector databaseSelector)
    {
        this.client = httpClientFactory.CreateClient();
        this.orientDbOptions = orientDbOptions;
        this.databaseSelector = databaseSelector;
        this.client.BaseAddress = orientDbOptions.CurrentValue.OrientDbServerUrl;

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        client.DefaultRequestHeaders.Add("Connection", "close");

        this.jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
    }

    public OrientDbHttpClient WithSession(IEnumerable<Claim> claims)
    {
        var orientDbSession = claims.First(x => x.Type == OrientDbClaims.OrientDbSession).Value;
        client.DefaultRequestHeaders.Add("Cookie", orientDbSession);

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
        var dbName = databaseName ?? databaseSelector.GetDatabaseName();
        var response = await client.GetAsync($"connect/{dbName}");

        if (!response.IsSuccessStatusCode)
        {
            response = await client.GetAsync($"connect/{dbName}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
        }

        var sessionId = OrientDbSession.ParseSessionId(response.Headers);

        return new OrientDbSession(orientDbOptions.CurrentValue.OrientDbServerUrl, sessionId, dbName!);
    }

    public async Task<OrientDbResult> QueryAsync(OrientDbCommand orientDbCommand)
    {
        var dbName = databaseSelector.GetDatabaseName();
        var url = $"/command/{dbName}/sql";

        var json = JsonSerializer.Serialize(orientDbCommand);
        using var httpContent = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json),
        };

        var response = await client.SendAsync(httpContent);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            response = await client.SendAsync(httpContent);
        }

        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();

        var orientDbResult = await JsonSerializer.DeserializeAsync<OrientDbResult>(stream, jsonOptions)
            ?? throw new Exception("Unable to parse query result to JsonElement");

        return orientDbResult;
    }

    public async Task<OrientDbDocumentResult> DocumentAsync(OrientDbId orientDbId)
    {
        var dbName = databaseSelector.GetDatabaseName();
        var url = $"/document/{dbName}/{orientDbId.FormatId()}";

        var response = await client.GetAsync(url);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            response = await client.GetAsync(url);
        }
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();

        var orientDbResult = await JsonSerializer.DeserializeAsync<OrientDbDocumentResult>(stream, jsonOptions)
            ?? throw new Exception("Unable to parse query result to JsonElement");

        return orientDbResult;
    }
}