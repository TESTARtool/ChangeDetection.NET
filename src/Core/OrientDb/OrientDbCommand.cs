namespace Testar.ChangeDetection.Core.OrientDb;

public sealed class OrientDbCommand
{
    public OrientDbCommand()
    {
        this.Command = string.Empty;
    }

    public OrientDbCommand(string sql)
    {
        this.Command = sql;
    }

    [Required]
    [JsonPropertyName("command")]
    public string Command { get; init; }

    [Required]
    [JsonPropertyName("parameters")]
    [JsonInclude]
    public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();

    public OrientDbCommand AddParameter(string name, string value)
    {
        if (!Command.Contains($":{name}"))
        {
            throw new ArgumentOutOfRangeException
            (
                paramName: nameof(name),
                message: $"Unable to find parameter in command with name '{name}'"
            );
        }

        Parameters.Add(name, value);
        return this;
    }
}