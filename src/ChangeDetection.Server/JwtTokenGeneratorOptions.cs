namespace Testar.ChangeDetection.Server;

public class JwtTokenGeneratorOptions
{
    public const string ConfigName = "JwtTokenGenerator";

    public string JwtSecurityKey { get; set; }
    public string JwtIssuer { get; set; }
    public string JwtAudience { get; set; }
    public int JwtExpiryInSeconds { get; set; }
}