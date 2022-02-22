using Blazored.LocalStorage;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Testar.ChangeDetection.Core;

public sealed class ChangeDetectionHttpClient : IChangeDetectionHttpClient
{
    private readonly JsonSerializerOptions jsonOptions;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILocalStorageService localStorageService;

    public ChangeDetectionHttpClient(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService)
    {
        this.jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        this.httpClientFactory = httpClientFactory;
        this.localStorageService = localStorageService;
    }

    public async Task<byte[]> DocumentAsync(OrientDbId id)
    {
        var httpClient = await CreateHttpClientAsync();
        var url = "/api/Document";
        var json = JsonSerializer.Serialize(id);
        using var httpContent = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await httpClient.SendAsync(httpContent);
        response.EnsureSuccessStatusCode();

        var value = await response.Content.ReadAsStringAsync();

        return string.IsNullOrWhiteSpace(value)
            ? Array.Empty<byte>()
            : Convert.FromBase64String(value);
    }

    public async Task<string?> LoginAsync(Uri serverUrl, LoginModel loginModel)
    {
        var httpClient = httpClientFactory.CreateClient();
        httpClient.BaseAddress = serverUrl;

        var loginAsJson = JsonSerializer.Serialize(loginModel);
        var response = await httpClient.PostAsync($"/api/Login", new StringContent(loginAsJson, Encoding.UTF8, "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var token = await response.Content.ReadAsStringAsync();

        return token;
    }

    public async Task<T[]> QueryAsync<T>(OrientDbCommand command)
    {
        try
        {
            var httpClient = await CreateHttpClientAsync();
            var url = $"/api/query";

            var json = JsonSerializer.Serialize(command);
            using var httpContent = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
            };

            var response = await httpClient.SendAsync(httpContent);

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();

            var orientDbResult = await JsonSerializer.DeserializeAsync<T[]>(stream, jsonOptions)
                ?? throw new Exception("Unable to parse query result to JsonElement");

            return orientDbResult;
        }
        catch (Exception)
        {
            Console.WriteLine("Something went wrong here: " + command.Command);

            throw;
        }
    }

    private async Task<HttpClient> CreateHttpClientAsync()
    {
        var httpClient = httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Clear();

        var location = await localStorageService.GetItemAsync<Uri>("serverLocation");
        if (location is not null)
        {
            httpClient.BaseAddress = location;
        }

        var authToken = await localStorageService.GetItemAsStringAsync("authToken");
        if (!string.IsNullOrWhiteSpace(authToken))
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        }

        return httpClient;
    }
}