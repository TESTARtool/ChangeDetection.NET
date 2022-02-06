using System.Net.Http.Headers;
using System.Security;
using System.Text;

namespace Testar.ChangeDetection.Core.OrientDb;

public interface IOrientDbLoginService
{
    Task<OrientDbSession> LoginAsync(Uri orientDb, string databaseName, string username, string password);

    Task DisconnectAsync(OrientDbSession session);
}

public sealed class OrientDbLoginService : IOrientDbLoginService
{
    private readonly ILogger<OrientDbLoginService> logger;
    private readonly IHttpClientFactory httpClientFactory;

    public OrientDbLoginService(ILogger<OrientDbLoginService> logger, IHttpClientFactory httpClientFactory)
    {
        this.logger = logger;
        this.httpClientFactory = httpClientFactory;
    }

    public async Task DisconnectAsync(OrientDbSession session)
    {
        var client = httpClientFactory.CreateOrientDbHttpClient(session.OrientDbUrl);
        client.DefaultRequestHeaders.Add("Cookie", session.SessionId);

        var response = await client.GetAsync("disconnect");

        logger.LogDisconnect(session.OrientDbUrl);
    }

    public async Task<OrientDbSession> LoginAsync(Uri orientDbUrl, string databaseName, string username, string password)
    {
        var client = httpClientFactory.CreateOrientDbHttpClient(orientDbUrl);

        var base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes($"{username}:{password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);

        var response = await client.GetAsync($"connect/{databaseName}");

        response.EnsureSuccessStatusCode();

        var sessionId = ParseSessionId(response.Headers);

        logger.LogSuccesfull(orientDbUrl, databaseName);
        logger.LogOrientDbCookie(sessionId);

        return new OrientDbSession(orientDbUrl, sessionId, databaseName);
    }

    internal static string ParseSessionId(HttpResponseHeaders headers)
    {
        // In the response header a cookie is set from the server
        // example: OSESSIONID=OS1644099715572-7340936222054740501; Path=/; HttpOnly

        var values = headers.GetValues("Set-Cookie")
            .FirstOrDefault()
            ?.Split(new[] { ';' });

        if (values is null || values.Length < 1)
        {
            throw new SecurityException("Did not receive a session id from OrientDb server");
        }

        return values[1].Trim();
    }
}