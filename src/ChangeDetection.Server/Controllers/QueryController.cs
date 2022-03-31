using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Testar.ChangeDetection.Server.OrientDb;

namespace Testar.ChangeDetection.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class QueryController : Controller
{
    private readonly ILogger<QueryController> logger;
    private readonly OrientDbHttpClient orientDbHttpClient;

    public QueryController(ILogger<QueryController> logger, OrientDbHttpClient orientDbHttpClient)
    {
        this.logger = logger;
        this.orientDbHttpClient = orientDbHttpClient;
    }

    /// <summary>
    /// Queries the database
    /// </summary>
    /// <param name="command"></param>
    /// <returns>JSON result</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/Query
    ///     {
    ///         "command": "SELECT FROM class where Id = :id",
    ///         {
    ///             "parameters" : {
    ///                 "id": "123"
    ///             }
    ///         }
    ///     }
    ///
    /// For every parameter and item in the parameters list must be added
    /// </remarks>
    [HttpPost]
    public async Task<IActionResult> Get([FromBody] OrientDbCommand command)
    {
        try
        {
            var result = await orientDbHttpClient
                .WithSession(User.Claims)
                .QueryAsync(command);

            return Ok(result.Result);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Get failed");

            return BadRequest(ex.Message);
        }
    }
}