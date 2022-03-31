using Blazored.LocalStorage;
using System.Security.Claims;

namespace BlazorApp;

public class User
{
    [Required]
    [Url]
    public string? ServerUrl { get; set; }

    [Required]
    public string? UserName { get; set; }

    [Required]
    public string? Password { get; set; }

    public bool RememberMe { get; set; }
}

public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService localStorage;

    public ApiAuthenticationStateProvider(ILocalStorageService localStorage)
    {
        this.localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var savedToken = await localStorage.GetItemAsStringAsync("authToken");
        var location = await localStorage.GetItemAsync<Uri>("serverLocation");

        if (string.IsNullOrWhiteSpace(savedToken) || location is null)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var jwt = new JwToken(savedToken);
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(jwt.Claims(), "jwt")));
    }

    public void MarkUserAsAuthenticated(JwToken token)
    {
        var authenticatedUser = new ClaimsPrincipal
        (
            new ClaimsIdentity(token.Claims(), "jwt")
        );
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
    }

    public void MarkUserAsLoggedOut()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(anonymousUser));
        NotifyAuthenticationStateChanged(authState);
    }
}