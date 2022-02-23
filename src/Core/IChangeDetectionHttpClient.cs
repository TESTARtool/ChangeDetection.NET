namespace Testar.ChangeDetection.Core;

public interface IChangeDetectionHttpClient
{
    Task<byte[]> DocumentAsync(OrientDbId id);

    Task<T[]> QueryAsync<T>(OrientDbCommand command);

    Task<string?> LoginAsync(Uri serverUrl, LoginModel loginModel);
}