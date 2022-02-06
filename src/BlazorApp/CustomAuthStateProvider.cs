using Blazored.LocalStorage;
using System.Security.Claims;

namespace BlazorApp;

public class User
{
    public string OrientDbUrl { get; set; }
    public string DatabaseName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService localStorageService;
    private readonly IOrientDbSessionProvider sessionProvider;

    public CustomAuthStateProvider(ILocalStorageService localStorageService, IOrientDbSessionProvider sessionProvider)
    {
        this.localStorageService = localStorageService;
        this.sessionProvider = sessionProvider;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var username = await localStorageService.GetItemAsync<string>("username");
        var session = await sessionProvider.GetSessionAsync();

        if (!string.IsNullOrWhiteSpace(username))
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            }, "OrientDb");

            var state = new AuthenticationState(new ClaimsPrincipal(identity));

            NotifyAuthenticationStateChanged(Task.FromResult(state));

            return state;
        }

        //var token = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiVG9ueSBTdGFyayIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6Iklyb24gTWFuIiwiZXhwIjozMTY4NTQwMDAwfQ.IbVQa1lNYYOzwso69xYfsMOHnQfO3VLvVqV2SOXS7sTtyyZ8DEf5jmmwz2FGLJJvZnQKZuieHnmHkg7CGkDbvA";

        //var claims = ParseClaimsFromJwt(token).ToList();

        ////var identity = new ClaimsIdentity(claims, "jwt");
        var nullIdentity = new ClaimsIdentity();
        var nullUser = new ClaimsPrincipal(nullIdentity);
        var nullState = new AuthenticationState(nullUser);

        NotifyAuthenticationStateChanged(Task.FromResult(nullState));

        return nullState;
    }

    //private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    //{
    //    var payload = jwt.Split('.')[1];
    //    var jsonBytes = ParseBase64WithoutPadding(payload);
    //    var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes)
    //        ?? throw new SecurityException("jtw token invalid");
    //    return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value?.ToString() ?? String.Empty));
    //}

    //private static byte[] ParseBase64WithoutPadding(string base64)
    //{
    //    switch (base64.Length % 4)
    //    {
    //        case 2: base64 += "=="; break;
    //        case 3: base64 += "="; break;
    //    }
    //    return Convert.FromBase64String(base64);
    //}
}