namespace Testar.ChangeDetection.Core;

public record struct AbstractActionId(string Value);
public record struct ModelIdentifier(string Value);
public record struct ConcreteStateId(string Value);
public record struct ConcreteActionId(string Value);
public record struct ConcreteID(string Value);
public record struct OrientDbId(string Id)
{
    public string FormatId() => Id?.Replace("#", "")?.Trim() ?? String.Empty;
}

public record struct AbstractStateId(string Value);
public record struct WidgetId(string Value);

public record struct SequenceId(string Value);