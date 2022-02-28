using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Testar.ChangeDetection.Server.JwToken;

public interface IGenerateJwTokens
{
    string GenerateToken(LoginModel login, string sessionId);
}

public class JwtTokenGenerator : IGenerateJwTokens
{
    private readonly IOptions<GeneratorOptions> options;

    public JwtTokenGenerator(IOptions<GeneratorOptions> options)
    {
        this.options = options;
    }

    public string GenerateToken(LoginModel login, string sessionId)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, login.Username),
            new Claim("OrientDbSession", sessionId),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.JwtSecurityKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.Now.AddSeconds(options.Value.JwtExpiryInSeconds);

        var token = new JwtSecurityToken(options.Value.JwtIssuer, options.Value.JwtAudience,
            claims,
            expires: expiry,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}