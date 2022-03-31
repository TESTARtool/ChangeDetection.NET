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
    private readonly ILogger<LoginController> logger;
    private readonly OrientDbHttpClient orientDbHttpClient;
    private readonly IOptions<OrientDbOptions> orientDbOptions;

    public LoginController(ILogger<LoginController> logger, OrientDbHttpClient orientDbHttpClient, IOptions<GeneratorOptions> options, IOptions<OrientDbOptions> orientDbOptions)
    {
        this.jwtOptions = options.Value;
        this.logger = logger;
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
        var (username, databaseName) = GetUsernameAndDatabaseName(login);

        if (orientDbOptions.Value.MultiDatabaseSupport && databaseName is null)
        {
            logger.LogWarning("Server support multiple database but client did not specified a databaseName");
            return BadRequest();
        }

        if (!orientDbOptions.Value.MultiDatabaseSupport && databaseName is not null)
        {
            logger.LogWarning("User tried signin to a database but server does not support multiple databases");
            return BadRequest();
        }

        var result = await orientDbHttpClient
        .WithUsernameAndPassword(username, login.Password)
        .LoginAsync(databaseName);

        if (result is null)
        {
            logger.LogWarning("OrientDb refused request");
            return Unauthorized();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(OrientDbClaims.OrientDbSession, result.SessionId),
        };

        if (databaseName is not null)
        {
            claims.Add(new Claim(OrientDbClaims.DatabaseName, databaseName));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.JwtSecurityKey));

        var token = new JwtSecurityToken(jwtOptions.JwtIssuer, jwtOptions.JwtAudience,
            claims,
            expires: DateTime.UtcNow.AddSeconds(jwtOptions.JwtExpiryInSeconds),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        var jsonToken = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(jsonToken);
    }

    public (string username, string? databaseName) GetUsernameAndDatabaseName(LoginModel login)
    {
        var splits = login.Username.Split(new[] { '/', '\\' });

        return splits.Length == 2
            ? (splits[1], splits[0])
            : (login.Username, null);
    }
}