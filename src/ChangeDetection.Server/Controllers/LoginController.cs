//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Options;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using Testar.ChangeDetection.Core;
//using Testar.ChangeDetection.Server.JwToken;
//using Testar.ChangeDetection.Server.OrientDb;

//namespace Testar.ChangeDetection.Server.Controllers;

//[Route("api/[controller]")]
//[ApiController]
//public class LoginController : Controller
//{
//    private readonly GeneratorOptions jwtOptions;
//    private readonly OrientDbHttpClient orientDbHttpClient;

//    public LoginController(OrientDbHttpClient orientDbHttpClient, IOptions<GeneratorOptions> options)
//    {
//        this.jwtOptions = options.Value;
//        this.orientDbHttpClient = orientDbHttpClient;
//    }

//    /// <summary>
//    /// Logins the user to the OrientDB database, returning a java web token (JTW)
//    /// </summary>
//    /// <param name="login"></param>
//    /// <returns>JWT</returns>
//    /// <remarks>
//    /// Sample request:
//    ///
//    ///     POST /api/Login
//    ///     {
//    ///         "username" : "orientDb user",
//    ///         "password" : "orientDb password"
//    ///     }
//    ///
//    /// </remarks>
//    [HttpPost]
//    public async Task<IActionResult> Login([FromBody] LoginModel login)
//    {
//        var result = await orientDbHttpClient
//            .WithUsernameAndPassword(login.Username, login.Password)
//            .LoginAsync(Database.StateDatabase);

//        if (result is null)
//        {
//            return BadRequest();
//        }

//        var claims = new[]
//        {
//            new Claim(ClaimTypes.Name, login.Username),
//            new Claim("OrientDbSession", result.SessionId),
//        };

//        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.JwtSecurityKey));
//        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
//        var expiry = DateTime.Now.AddSeconds(jwtOptions.JwtExpiryInSeconds);

//        var token = new JwtSecurityToken(jwtOptions.JwtIssuer, jwtOptions.JwtAudience,
//            claims,
//            expires: expiry,
//            signingCredentials: creds
//        );

//        var jsonToken = new JwtSecurityTokenHandler().WriteToken(token);

//        return Ok(jsonToken);
//    }
//}