using Blazored.LocalStorage;
using System.Security.Claims;
using System.Text.Json;
using Testar.ChangeDetection.Core.Graph;

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
    private readonly ChangeDetectionHttpClient httpClient;
    private readonly ILocalStorageService localStorage;

    public ApiAuthenticationStateProvider(ChangeDetectionHttpClient httpClient, ILocalStorageService localStorage)
    {
        this.httpClient = httpClient;
        this.localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var savedToken = await localStorage.GetItemAsync<string>("authToken");
        var location = await localStorage.GetItemAsync<Uri>("serverLocation");

        if (string.IsNullOrWhiteSpace(savedToken) || location is null)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        httpClient.SetAuthenticationToken(savedToken);
        httpClient.SetBaseAddress(location);

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(savedToken), "jwt")));
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

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);

        if (roles != null)
        {
            if (roles.ToString().Trim().StartsWith("["))
            {
                var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                foreach (var parsedRole in parsedRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                }
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
            }

            keyValuePairs.Remove(ClaimTypes.Role);
        }

        claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

        return claims;
    }

    private byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}