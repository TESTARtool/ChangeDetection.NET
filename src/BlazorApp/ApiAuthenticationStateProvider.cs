using Blazored.LocalStorage;
using System.Security.Claims;
using System.Text.Json;

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

        var claims = ParseClaimsFromJwt(savedToken);
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
    }

    public void MarkUserAsAuthenticated(string username)
    {
        var authenticatedUser = new ClaimsPrincipal(
            new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }, "apiauth"));
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
    }

    public void MarkUserAsLoggedOut()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(anonymousUser));
        NotifyAuthenticationStateChanged(authState);
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes) ?? new Dictionary<string, object>();
       
        claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value?.ToString() ?? string.Empty)));

        return claims;
    }
}