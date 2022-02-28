using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Testar.ChangeDetection.Server.OrientDb;

public class OrientDbHealthCheck : IHealthCheck
{
    private readonly IOptionsMonitor<OrientDbOptions> orientDbOptions;

    public OrientDbHealthCheck(IOptionsMonitor<OrientDbOptions> orientDbOptions)
    {
        this.orientDbOptions = orientDbOptions;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var databaseName = orientDbOptions.CurrentValue.StateDatabaseName;

        var httpClient = new HttpClient()
        {
            BaseAddress = orientDbOptions.CurrentValue.OrientDbServerUrl
        };

        var response = await httpClient.GetAsync($"connect/{databaseName}");

        return response.StatusCode == System.Net.HttpStatusCode.Unauthorized
            ? new HealthCheckResult(HealthStatus.Healthy)
            : new HealthCheckResult(HealthStatus.Unhealthy, "Unable to connet to Orient DB");
    }
}

public static class OrientDbHealthCheckExtensions
{
    public static void AddOrientDbHealthCheck(this IHealthChecksBuilder builder)
    {
        builder.AddCheck<OrientDbHealthCheck>("Orient DB Health Check");
    }
}