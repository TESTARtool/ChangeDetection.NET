namespace Testar.ChangeDetection.ConsoleApp;

public class TestarServerOptions
{
    public const string ConfigName = "TestarServer";

    public Uri Url { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}