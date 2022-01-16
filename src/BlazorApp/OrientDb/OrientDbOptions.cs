namespace Testar.ChangeDetection.BlazorApp.OrientDb;

public class OrientDbOptions
{
    public const string ConfigName = "OrientDB";

    public Uri Url { get; set; }
    public string DatabaseName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}