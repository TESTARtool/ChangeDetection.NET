using Blazored.LocalStorage;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Testar.ChangeDetection.Core;

namespace BlazorApp;

public interface IAuthService
{
    Task<bool> LoginAsync(User user);

    Task LogoutAsync();
}

public class AuthService : IAuthService
{
    private readonly HttpClient httpClient;
    private readonly AuthenticationStateProvider authenticationStateProvider;
    private readonly ILocalStorageService localStorage;

    public AuthService(HttpClient httpClient,
        AuthenticationStateProvider authenticationStateProvider,
        ILocalStorageService localStorage)
    {
        this.httpClient = httpClient;
        this.authenticationStateProvider = authenticationStateProvider;
        this.localStorage = localStorage;
    }

    public async Task<bool> LoginAsync(User user)
    {
        if (user.UserName is not null && user.Password is not null)
        {
            var loginAsJson = JsonSerializer.Serialize(new LoginModel
            {
                Username = user.UserName,
                Password = user.Password
            });

            var response = await httpClient.PostAsync($"{user.ServerUrl}/api/Login", new StringContent(loginAsJson, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var token = await response.Content.ReadAsStringAsync();

            await localStorage.SetItemAsync("authToken", token);
            ((ApiAuthenticationStateProvider)authenticationStateProvider).MarkUserAsAuthenticated(user.UserName);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            return true;
        }

        return false;
    }

    public async Task LogoutAsync()
    {
        await localStorage.RemoveItemAsync("authToken");
        ((ApiAuthenticationStateProvider)authenticationStateProvider).MarkUserAsLoggedOut();
        httpClient.DefaultRequestHeaders.Authorization = null;
    }
}