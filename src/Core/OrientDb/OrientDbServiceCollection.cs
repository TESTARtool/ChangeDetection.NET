using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace Testar.ChangeDetection.Core.OrientDb;

public static class OrientDbServiceCollection
{
    public static IServiceCollection AddOrientDb<TSessionProvider>(this IServiceCollection services)
        where TSessionProvider : class, IOrientDbSessionProvider
    {
        services
            .AddScoped<IOrientDbCommand, OrientDbCommand>()
            .AddScoped<IOrientDbLoginService, OrientDbLoginService>()
            .AddScoped(typeof(IOrientDbSessionProvider), typeof(TSessionProvider))
            ;

        services.AddHttpClient(OrientDbHttpClientFactory.OrientDbHttpClient, httpClient =>
        {
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        });

        return services;
    }
}