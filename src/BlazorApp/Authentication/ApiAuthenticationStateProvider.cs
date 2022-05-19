using Blazored.LocalStorage;
using System.Security.Claims;

namespace BlazorApp.Authentication;

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
    private readonly ILogger<ApiAuthenticationStateProvider> logger;
    private readonly ILocalStorageService localStorage;

    public ApiAuthenticationStateProvider(ILogger<ApiAuthenticationStateProvider> logger, ILocalStorageService localStorage)
    {
        this.logger = logger;
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

        try
        {
            var jwt = new JwToken(savedToken);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(jwt.Claims(), "jwt")));
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "JWT error - Assume unauthenticated user");
            await localStorage.RemoveItemAsync("authToken");
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            return new AuthenticationState(anonymousUser);
        }
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