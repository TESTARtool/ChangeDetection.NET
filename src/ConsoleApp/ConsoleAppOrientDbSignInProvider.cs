using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;

namespace Testar.ChangeDetection.ConsoleApp;

public class ConsoleAppOrientDbSignInProvider : IOrientDbSignInProvider
{
    private readonly OrientDbOptions options;

    public ConsoleAppOrientDbSignInProvider(IOptions<OrientDbOptions> options)
    {
        this.options = options.Value;
    }

    public Task EnhanceHttpClient(HttpClient httpClient)
    {
        var base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes($"{options.Username}:{options.Password}"));

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
        httpClient.DefaultRequestHeaders.Add("Connection", "close");

        httpClient.BaseAddress = options.Url;

        return Task.CompletedTask;
    }

    public Task<string?> GetDatabaseNameAsync()
    {
        return Task.FromResult((string?)options.DatabaseName);
    }
}