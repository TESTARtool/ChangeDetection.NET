using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Testar.ChangeDetection.Server.JwToken;
using Testar.ChangeDetection.Server.OrientDb;

namespace Testar.ChangeDetection.Server;

public class OptionsHealthCheck : IHealthCheck
{
    private readonly IOptionsMonitor<GeneratorOptions> generatorOptions;
    private readonly IOptionsMonitor<OrientDbOptions> orientDbOptions;

    public OptionsHealthCheck(IOptionsMonitor<GeneratorOptions> generatorOptions,
        IOptionsMonitor<OrientDbOptions> orientDbOptions)
    {
        this.generatorOptions = generatorOptions;
        this.orientDbOptions = orientDbOptions;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var jwtOptions = generatorOptions.CurrentValue;

        if (string.IsNullOrWhiteSpace(jwtOptions.JwtAudience))
        {
            return Task.FromResult(HealthCheckResult.Unhealthy($"{nameof(jwtOptions.JwtAudience)} is null or empty"));
        }

        if (string.IsNullOrWhiteSpace(jwtOptions.JwtSecurityKey))
        {
            return Task.FromResult(HealthCheckResult.Unhealthy($"{nameof(jwtOptions.JwtSecurityKey)} is null or empty"));
        }

        if (string.IsNullOrWhiteSpace(jwtOptions.JwtIssuer))
        {
            return Task.FromResult(HealthCheckResult.Unhealthy($"{nameof(jwtOptions.JwtIssuer)} is null or empty"));
        }

        if (jwtOptions.JwtExpiryInSeconds < 1)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy($"{nameof(jwtOptions.JwtExpiryInSeconds)} is lower than 1"));
        }

        var orientOptions = orientDbOptions.CurrentValue;

        if (!orientDbOptions.CurrentValue.MultiDatabaseSupport && string.IsNullOrWhiteSpace(orientOptions.StateDatabaseName))
        {
            return Task.FromResult(HealthCheckResult.Unhealthy($"{nameof(orientOptions.StateDatabaseName)} is null or empty"));
        }

        if (orientOptions.OrientDbServerUrl is null || string.IsNullOrWhiteSpace(orientOptions.OrientDbServerUrl.ToString()))
        {
            return Task.FromResult(HealthCheckResult.Unhealthy($"{nameof(orientOptions.OrientDbServerUrl)} is null or empty"));
        }

        return Task.FromResult(HealthCheckResult.Healthy());
    }
}