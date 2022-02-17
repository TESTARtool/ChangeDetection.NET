using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Testar.ChangeDetection.Core;

namespace ChangeDetection.Server.Controllers;

public class JwtTokenGeneratorOptions
{
    public const string ConfigName = "JwtTokenGenerator";

    public string JwtSecurityKey { get; set; }
    public string JwtIssuer { get; set; }
    public string JwtAudience { get; set; }
    public int JwtExpiryInSeconds { get; set; }
}

[Route("api/[controller]")]
[ApiController]
public class LoginController : Controller
{
    private readonly IConfiguration configuration;
    private readonly IOrientDbLoginService orientDbLoginService;
    private readonly JwtTokenGeneratorOptions options;

    public LoginController(IConfiguration configuration, IOrientDbLoginService orientDbLoginService,
        IOptions<JwtTokenGeneratorOptions> options)
    {
        this.configuration = configuration;
        this.orientDbLoginService = orientDbLoginService;
        this.options = options.Value;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginModel login)
    {
        var orientDb = configuration["OrientDbServerUrl"];
        var databaseName = configuration["StateDatabaseName"];

        var result = await orientDbLoginService.LoginAsync(new Uri(orientDb), databaseName, login.Username, login.Password);

        if (result is null)
        {
            return BadRequest();
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, login.Username),
            new Claim("OrientDbSession", result.SessionId),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.JwtSecurityKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.Now.AddSeconds(options.JwtExpiryInSeconds);

        var token = new JwtSecurityToken(

            options.JwtIssuer,
            options.JwtAudience,
            claims,
            expires: expiry,
            signingCredentials: creds
        );

        var jsonToken = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(jsonToken);
    }
}