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

    /// <summary>
    /// Gets a document from OrientDB
    /// </summary>
    /// <param name="orientDbId">Orient DB Id of the document</param>
    /// <returns>Base64 representation of the document</returns>
    /// <remarks>
    /// Sample request
    ///
    ///     GET /api/Document/4:1
    ///
    /// </remarks>
    [HttpGet("{Id}")]
    public async Task<IActionResult> Get(string id)
    {
        var result = await orientDbHttpClient
            .WithSession(User.Claims)
            .DocumentAsync(new OrientDbId(id), Database.StateDatabase);

        return Ok(result.Value);
    }
}