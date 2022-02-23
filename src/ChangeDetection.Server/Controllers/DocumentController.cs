using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Testar.ChangeDetection.Core;
using Testar.ChangeDetection.Server.OrientDb;

namespace Testar.ChangeDetection.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DocumentController : Controller
{
    private readonly OrientDbHttpClient orientDbHttpClient;

    public DocumentController(OrientDbHttpClient orientDbHttpClient)
    {
        this.orientDbHttpClient = orientDbHttpClient;
    }

    [HttpPost]
    public async Task<IActionResult> Get([FromBody] OrientDbId orientDbId)
    {
        var result = await orientDbHttpClient
            .WithSession(User.Claims)
            .DocumentAsync(orientDbId, Database.StateDatabase);

        return Ok(result.Value);
    }
}