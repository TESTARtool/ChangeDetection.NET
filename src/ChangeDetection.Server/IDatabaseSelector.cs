using Microsoft.Extensions.Options;
using System.Security.Claims;
using Testar.ChangeDetection.Core;
using Testar.ChangeDetection.Server.OrientDb;

namespace Testar.ChangeDetection.Server;

public interface IDatabaseSelector
{
    string GetDatabaseName();
}

public class ClaimsDatabaseSelector : IDatabaseSelector
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public ClaimsDatabaseSelector(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public string GetDatabaseName()
    {
        var httpContext = httpContextAccessor.HttpContext ?? throw new InvalidOperationException("Unable to retrieve http context");
        var hasDatabaseClaim = httpContext.User.HasClaim(x => x.Type == OrientDbClaims.DatabaseName);
        if (!hasDatabaseClaim)
        {
            throw new InvalidOperationException($"Unable to find a claim with type {OrientDbClaims.DatabaseName}");
        }

        return httpContext.User.FindFirstValue(OrientDbClaims.DatabaseName);
    }
}

public class OptionsDatabaseSelector : IDatabaseSelector
{
    private readonly OrientDbOptions orientDbOptions;

    public OptionsDatabaseSelector(IOptions<OrientDbOptions> orientDbOptions)
    {
        this.orientDbOptions = orientDbOptions.Value;
    }

    public string GetDatabaseName() => orientDbOptions.StateDatabaseName
        ?? throw new InvalidOperationException("DatabaseName not found in settings");
}