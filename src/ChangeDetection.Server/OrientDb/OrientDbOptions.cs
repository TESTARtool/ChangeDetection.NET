namespace Testar.ChangeDetection.Server.OrientDb;

public class OrientDbOptions
{
    public const string ConfigName = "OrientDb";

    public Uri OrientDbServerUrl { get; set; }
    public string? StateDatabaseName { get; set; }

    public bool MultiDatabaseSupport { get; set; }
}