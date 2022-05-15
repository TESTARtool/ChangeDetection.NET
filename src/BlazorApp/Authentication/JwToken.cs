using System.Security.Claims;
using System.Text.Json;

namespace BlazorApp.Authentication;

public class JwToken
{
    private readonly string token;

    public JwToken(string token)
    {
        this.token = token;
    }

    public IReadOnlyCollection<Claim> Claims()
    {
        var payload = token.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes) ?? new Dictionary<string, object>();

        return keyValuePairs
            .Select(kvp => new Claim(kvp.Key, kvp.Value?.ToString() ?? string.Empty))
            .ToList();
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
}