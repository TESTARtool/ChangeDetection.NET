using System.Diagnostics;
using System.Xml.Serialization;

namespace Testar.ChangeDetection.Core.Strategy.WidgetTreeInitialState;

[DebuggerDisplay("{Role}-{Title}")]
public class Widget
{
    [XmlAttribute]
    public string Role { get; init; }

    [XmlAttribute]
    public string Title { get; init; }

    public OrientDbId[] InIsChildOf { get; init; }
    public OrientDbId[] OutIsChildOf { get; init; }
    public List<Widget> Children { get; } = new();
    public Dictionary<string, string> Properties { get; } = new();
}