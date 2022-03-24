using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Testar.ChangeDetection.Core;
using Testar.ChangeDetection.Server.JwToken;
using Testar.ChangeDetection.Server.OrientDb;

namespace Testar.ChangeDetection.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoginController : Controller
{
    private readonly GeneratorOptions jwtOptions;
    private readonly OrientDbHttpClient orientDbHttpClient;
    private readonly IOptions<OrientDbOptions> orientDbOptions;

    public LoginController(OrientDbHttpClient orientDbHttpClient, IOptions<GeneratorOptions> options, IOptions<OrientDbOptions> orientDbOptions)
    {
        this.jwtOptions = options.Value;
        this.orientDbHttpClient = orientDbHttpClient;
        this.orientDbOptions = orientDbOptions;
    }

    /// <summary>
    /// Logins the user to the OrientDB database, returning a java web token (JTW)
    /// </summary>
    /// <param name="login"></param>
    /// <returns>JWT</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/Login
    ///     {
    ///         "username" : "orientDb username",
    ///         "password" : "orientDb password"
    ///     }
    ///
    /// </remarks>
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginModel login)
    {
        var username = login.Username.Split(new[] { '/', '\\' });
        string? databaseName = null;
        if (username.Length > 1)
        {
            databaseName = username[0];
            username = new[] { username[1] };
        }

        if ((orientDbOptions.Value.MultiDatabaseSupport && databaseName is null) ||
           (!orientDbOptions.Value.MultiDatabaseSupport && databaseName is not null))
        {
            return BadRequest();
        }

        var result = await orientDbHttpClient
            .WithUsernameAndPassword(username[0], login.Password)
            .LoginAsync(databaseName);

        if (result is null)
        {
            return BadRequest();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username[0]),
            new Claim("OrientDbSession", result.SessionId),
        };

        if (databaseName is not null)
        {
            claims.Add(new Claim("DatabaseName", databaseName));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.JwtSecurityKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.Now.AddSeconds(jwtOptions.JwtExpiryInSeconds);

        var token = new JwtSecurityToken(jwtOptions.JwtIssuer, jwtOptions.JwtAudience,
            claims,
            expires: expiry,
            signingCredentials: creds
        );

        var jsonToken = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(jsonToken);
    }
}