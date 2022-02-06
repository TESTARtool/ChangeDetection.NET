using Blazored.LocalStorage;

internal class BlazorOrientDbSessionProvider : IOrientDbSessionProvider
{
    private readonly ILocalStorageService localStorageService;

    public BlazorOrientDbSessionProvider(ILocalStorageService localStorageService)
    {
        this.localStorageService = localStorageService;
    }

    public async Task<OrientDbSession> GetSessionAsync()
    {
        var url = await localStorageService.GetItemAsStringAsync("orientdb-url");
        var session = await localStorageService.GetItemAsStringAsync("orientdb-session");
        var databaseName = await localStorageService.GetItemAsStringAsync("testar-database");

        return new OrientDbSession(new Uri(url), session, databaseName);
    }
}