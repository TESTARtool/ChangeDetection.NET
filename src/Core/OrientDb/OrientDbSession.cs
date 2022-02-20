using System.Net.Http.Headers;
using System.Security;

namespace Testar.ChangeDetection.Core.OrientDb;

public record OrientDbSession(Uri OrientDbUrl, string SessionId, string DatabaseName)
{
    public static string ParseSessionId(HttpResponseHeaders headers)
    {
        // In the response header a cookie is set from the orientdb server
        // example: OSESSIONID=OS1644099715572-7340936222054740501; Path=/; HttpOnly

        var values = headers.GetValues("Set-Cookie")
            .FirstOrDefault()
            ?.Split(new[] { ';' });

        if (values is null || values.Length < 1)
        {
            throw new SecurityException("Did not receive a session id from OrientDb server");
        }

        return values[0].Trim();
    }
}