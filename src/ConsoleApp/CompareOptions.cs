namespace Testar.ChangeDetection.ConsoleApp;

public class CompareOptions
{
    public const string ConfigName = "Compare";

    public string ControlName { get; set; }
    public string ControlVersion { get; set; }

    public string TestName { get; set; }
    public string TestVersion { get; set; }
}

public class OrientDbOptions
{
    public const string ConfigName = "OrientDb";

    public Uri Url { get; set; }
    public string DatabaseName { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}