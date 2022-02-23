namespace Testar.ChangeDetection.Server.JwToken;

public class GeneratorOptions
{
    public const string ConfigName = "JwTokenGenerator";

    public string JwtSecurityKey { get; set; }
    public string JwtIssuer { get; set; }
    public string JwtAudience { get; set; }
    public int JwtExpiryInSeconds { get; set; }
}