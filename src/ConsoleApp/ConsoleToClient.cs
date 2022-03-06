﻿using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Testar.ChangeDetection.Core;

namespace Testar.ChangeDetection.ConsoleApp;

public class ConsoleToClient : IChangeDetectionHttpClient
{
    private readonly JsonSerializerOptions jsonOptions;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IOptions<TestarServerOptions> orientDbOptions;
    private string? authToken = null;

    public ConsoleToClient(IHttpClientFactory httpClientFactory, IOptions<TestarServerOptions> orientDbOptions)
    {
        this.jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        this.httpClientFactory = httpClientFactory;
        this.orientDbOptions = orientDbOptions;
    }

    public async Task<byte[]> DocumentAsync(OrientDbId id)
    {
        var httpClient = CreateHttpClient();
        var url = $"/api/Document/{id.FormatId()}";

        var response = await httpClient.GetAsync(url);

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
            authToken = null;
            return null;
        }

        authToken = await response.Content.ReadAsStringAsync();

        return authToken;
    }

    public async Task<T[]> QueryAsync<T>(OrientDbCommand command)
    {
        var httpClient = CreateHttpClient();
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

    private HttpClient CreateHttpClient()
    {
        var httpClient = httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Clear();

        if (httpClient.BaseAddress is null)
        {
            httpClient.BaseAddress = orientDbOptions.Value.Url;
        }

        if (!string.IsNullOrWhiteSpace(authToken))
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        }

        return httpClient;
    }
}