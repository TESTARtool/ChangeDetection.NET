namespace Testar.ChangeDetection.ConsoleApp;

public class CompareOptions
{
    public const string ConfigName = "Compare";

    public string ControlName { get; set; }
    public string ControlVersion { get; set; }

    public string TestName { get; set; }
    public string TestVersion { get; set; }
}