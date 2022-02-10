using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace Testar.ChangeDetection.Core.OrientDb;

public static class OrientDbServiceCollection
{
    public static IServiceCollection AddOrientDb(this IServiceCollection services)
    {
        services
            .AddScoped<IOrientDbCommand, OrientDbCommand>()
            .AddScoped<IOrientDbLoginService, OrientDbLoginService>()
            ;

        services.AddHttpClient(OrientDbHttpClientFactory.OrientDbHttpClient, (services, httpClient) =>
        {
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        });

        return services;
    }
}