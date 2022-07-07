using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Testar.ChangeDetection.Core;

internal class ConsoleChangeDetectionHttpClient : IChangeDetectionHttpClient
{
    private readonly JsonSerializerOptions jsonOptions;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly DataContainer dataContainer;

    public ConsoleChangeDetectionHttpClient(IHttpClientFactory httpClientFactory, DataContainer dataContainer)
    {
        this.jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        this.httpClientFactory = httpClientFactory;
        this.dataContainer = dataContainer;
    }

    public async Task<string?> DocumentAsBase64Async(OrientDbId id)
    {
        var httpClient = CreateHttpClientAsync();
        var url = $"/api/Document/{id.FormatId()}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);

        var response = await httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var value = await response.Content.ReadAsStringAsync();

        return value;
    }

    public async Task<byte[]> DocumentAsync(OrientDbId id)
    {
        var value = await DocumentAsBase64Async(id);

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

        dataContainer.AuthToken = await response.Content.ReadAsStringAsync();

        return dataContainer.AuthToken;
    }

    public async Task<T[]> QueryAsync<T>(OrientDbCommand command)
    {
        var httpClient = CreateHttpClientAsync();

        try
        {
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
        catch (HttpRequestException)
        {
            return Array.Empty<T>();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Something went wrong here: " + command.Command);

            throw;
        }
    }

    private HttpClient CreateHttpClientAsync()
    {
        var httpClient = httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Clear();

        httpClient.BaseAddress = new Uri(dataContainer.ServerUrl);

        var authToken = dataContainer.AuthToken;
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        return httpClient;
    }
}