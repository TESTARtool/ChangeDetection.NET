using Blazored.LocalStorage;
using System.Net.Http.Headers;

internal sealed class BlazorOrientDbSignInProvider : IOrientDbSignInProvider
{
    private readonly ILocalStorageService localStorageService;

    public BlazorOrientDbSignInProvider(ILocalStorageService localStorageService)
    {
        this.localStorageService = localStorageService;
    }

    public async Task EnhanceHttpClient(HttpClient httpClient)
    {
        var session = await localStorageService.GetItemAsStringAsync("session");
        var url = await localStorageService.GetItemAsync<Uri>("orientdb-url");

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", session);
        httpClient.DefaultRequestHeaders.Add("Connection", "close");

        httpClient.BaseAddress = url;
    }

    public async Task<string?> GetDatabaseNameAsync()
    {
        var databaseName = await localStorageService.GetItemAsStringAsync("testar-database");
        return databaseName;
    }
}