internal class CustomOrientDbSessionProvider : IOrientDbSessionProvider
{
    private OrientDbSession session;

    public Task<OrientDbSession> GetSessionAsync()
    {
        return Task.FromResult(session);
    }

    public void SetSession(OrientDbSession session)
    {
        this.session = session;
    }
}