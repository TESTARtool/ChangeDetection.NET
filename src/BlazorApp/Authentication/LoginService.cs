using Blazored.LocalStorage;
using Testar.ChangeDetection.Core;

namespace BlazorApp.Authentication;

public interface IAuthService
{
    Task<bool> LoginAsync(User user);

    Task LogoutAsync();
}

public class AuthService : IAuthService
{
    private readonly IChangeDetectionHttpClient changeDetectionHttpClient;
    private readonly AuthenticationStateProvider authenticationStateProvider;
    private readonly ILocalStorageService localStorage;
    private readonly ILogger<AuthService> logger;

    public AuthService(IChangeDetectionHttpClient changeDetectionHttpClient,
        AuthenticationStateProvider authenticationStateProvider,
        ILocalStorageService localStorage,
        ILogger<AuthService> logger
        )
    {
        this.changeDetectionHttpClient = changeDetectionHttpClient;
        this.authenticationStateProvider = authenticationStateProvider;
        this.localStorage = localStorage;
        this.logger = logger;
    }

    public async Task<bool> LoginAsync(User user)
    {
        try
        {
            if (user.UserName is not null && user.Password is not null && user.ServerUrl is not null)
            {
                var url = user.ServerUrl.EndsWith('/')
                    ? new Uri(user.ServerUrl.Substring(0, user.ServerUrl.Length - 1))
                    : new Uri(user.ServerUrl);

                var token = await changeDetectionHttpClient.LoginAsync(url, new LoginModel
                {
                    Username = user.UserName,
                    Password = user.Password
                });

                if (token is null)
                {
                    return false;
                }

                await localStorage.SetItemAsStringAsync("authToken", token);
                await localStorage.SetItemAsync("serverLocation", new Uri(user.ServerUrl));

                ((ApiAuthenticationStateProvider)authenticationStateProvider).MarkUserAsAuthenticated(new JwToken(token));

                return true;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Login failed");
        }
        return false;
    }

    public async Task LogoutAsync()
    {
        await localStorage.RemoveItemAsync("authToken");
        ((ApiAuthenticationStateProvider)authenticationStateProvider).MarkUserAsLoggedOut();
    }
}