using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Testar.ChangeDetection.Core;

namespace Testar.ChangeDetection.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class QueryController : Controller
{
    private readonly OrientDbHttpClient orientDbHttpClient;

    public QueryController(OrientDbHttpClient orientDbHttpClient)
    {
        this.orientDbHttpClient = orientDbHttpClient;
    }

    [HttpPost]
    public async Task<IActionResult> Query([FromBody] OrientDbCommand command)
    {
        var result = await orientDbHttpClient
            .WithSession(User.Claims)
            .QueryAsync(command, Database.StateDatabase);

        return Ok(result.Result);
    }

    [HttpGet]
    public async Task<IActionResult> Document([FromBody] OrientDbId orientDbId)
    {
        var result = await orientDbHttpClient
            .WithSession(User.Claims)
            .DocumentAsync(orientDbId, Database.CompareDatabase);

        return Ok(result.Value);
    }
}