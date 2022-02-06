namespace Testar.ChangeDetection.Core.OrientDb;

public interface IOrientDbSessionProvider
{
    Task<OrientDbSession> GetSessionAsync();
}
