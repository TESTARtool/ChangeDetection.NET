namespace Testar.ChangeDetection.Core.Graph;

public class Vertex : Document
{
    public Vertex(string id) : base(id)
    {
    }

    [JsonIgnore]
    public string? StateId => base.Properties.TryGetValue("stateId", out object? value) ? value.ToString() : null;
}