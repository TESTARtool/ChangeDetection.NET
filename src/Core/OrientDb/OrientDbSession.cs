namespace Testar.ChangeDetection.Core.OrientDb;

public record OrientDbSession(Uri OrientDbUrl, string SessionId, string DatabaseName);