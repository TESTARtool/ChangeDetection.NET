internal class CustomOrientDbSessionProvider : IOrientDbSessionProvider
{
    private OrientDbSession? session = null;

    public Task<OrientDbSession?> GetSessionAsync()
    {
        return Task.FromResult(session);
    }

    public void SetSession(OrientDbSession session)
    {
        this.session = session;
    }
}