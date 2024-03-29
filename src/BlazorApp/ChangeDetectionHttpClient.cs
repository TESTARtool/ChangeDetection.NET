﻿using Blazored.LocalStorage;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Testar.ChangeDetection.Core;

public sealed class ChangeDetectionHttpClient : IChangeDetectionHttpClient
{
    private readonly JsonSerializerOptions jsonOptions;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILocalStorageService localStorageService;
    private readonly NavigationManager navigationManager;
    private readonly IToastService toastService;

    public ChangeDetectionHttpClient(IHttpClientFactory httpClientFactory,
        ILocalStorageService localStorageService,
        NavigationManager navigationManager,
        Blazored.Toast.Services.IToastService toastService
        )
    {
        this.jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        this.httpClientFactory = httpClientFactory;
        this.localStorageService = localStorageService;
        this.navigationManager = navigationManager;
        this.toastService = toastService;
    }

    public async Task<string?> DocumentAsBase64Async(OrientDbId id)
    {
        var httpClient = await CreateHttpClientAsync();
        var url = $"/api/Document/{id.FormatId()}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.SetBrowserRequestCache(BrowserRequestCache.ForceCache);

        var response = await httpClient.SendAsync(request);

        await EnsureSuccessStatusCodeAsync(response);

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

        var token = await response.Content.ReadAsStringAsync();

        return token;
    }

    public async Task<T[]> QueryAsync<T>(OrientDbCommand command)
    {
        var httpClient = await CreateHttpClientAsync();

        try
        {
            var url = $"/api/query";

            var json = JsonSerializer.Serialize(command);
            using var httpContent = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
            };

            var response = await httpClient.SendAsync(httpContent);

            await EnsureSuccessStatusCodeAsync(response);

            using var stream = await response.Content.ReadAsStreamAsync();

            var orientDbResult = await JsonSerializer.DeserializeAsync<T[]>(stream, jsonOptions)
                ?? throw new Exception("Unable to parse query result to JsonElement");

            return orientDbResult;
        }
        catch (HttpRequestException)
        {
            toastService.ShowError("The TESTAR .NET Server cannot be reached.", "404 - TESTAR .NET Server");
            return Array.Empty<T>();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Something went wrong here: " + command.Command);

            throw;
        }
    }

    public async Task<string> QueryRaw(OrientDbCommand command)
    {
        var httpClient = await CreateHttpClientAsync();

        try
        {
            var url = $"/api/query";

            var json = JsonSerializer.Serialize(command);
            using var httpContent = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
            };

            var response = await httpClient.SendAsync(httpContent);

            await EnsureSuccessStatusCodeAsync(response);

            return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException)
        {
            toastService.ShowError("The TESTAR .NET Server cannot be reached.", "404 - TESTAR .NET Server");
            return String.Empty;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    private async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)
    {
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            toastService.ShowError("The TESTAR .NET Server cannot be reached.", "404 - TESTAR .NET Server");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            await localStorageService.RemoveItemAsync("authToken");

            navigationManager.NavigateTo($"/login?returnUrl={navigationManager.Uri}", forceLoad: true, replace: true);
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
        else
        {
            navigationManager.NavigateTo($"/login?returnUrl={navigationManager.Uri}", forceLoad: true, replace: true);

            throw new InvalidOperationException("Sign in required");
        }

        var authToken = await localStorageService.GetItemAsStringAsync("authToken");
        if (!string.IsNullOrWhiteSpace(authToken))
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        }

        return httpClient;
    }
}