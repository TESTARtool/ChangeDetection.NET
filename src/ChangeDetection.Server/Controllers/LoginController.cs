using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChangeDetection.Server.Controllers;

public class LoginModel
{
    public string Username { get; set; }

    public string Password { get; set; }

}

[Route("api/[controller]")]
[ApiController]
public class LoginController : Controller
{
    private readonly IConfiguration configuration;
    private readonly IOrientDbLoginService orientDbLoginService;

    public LoginController(IConfiguration configuration, IOrientDbLoginService orientDbLoginService)
    {
        this.configuration = configuration;
        this.orientDbLoginService = orientDbLoginService;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginModel login)
    {
        var orientDb = configuration["OrientDbServerUrl"];
        var result = await orientDbLoginService.LoginAsync(new Uri(orientDb), "testar2", login.Username, login.Password);

        if (result is null)
        {
            return BadRequest();
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, login.Username),
            new Claim("OrientDbSession", result.SessionId),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSecurityKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // todo may the lower
        var expiry = DateTime.Now.AddDays(Convert.ToInt32(configuration["JwtExpiryInDays"]));

        var token = new JwtSecurityToken(
            configuration["JwtIssuer"],
            configuration["JwtAudience"],
            claims,
            expires: expiry,
            signingCredentials: creds
        );

        return Ok(new JwtSecurityTokenHandler().WriteToken(token));
    }
}