using System.Text.Json;

namespace Testar.ChangeDetection.Server.OrientDb;

public sealed class OrientDbResult
{
    [JsonPropertyName("result")]
    public JsonElement Result { get; set; }
}
