namespace Testar.ChangeDetection.Core.OrientDb;

public static class OrientDbHttpClientFactory
{
    public static readonly string OrientDbHttpClient = nameof(OrientDbHttpClient);

    public static HttpClient CreateOrientDbHttpClient(this IHttpClientFactory factory)
    {
        var client = factory.CreateClient(OrientDbHttpClient);

        return client;
    }
}