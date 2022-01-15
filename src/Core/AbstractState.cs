namespace Testar.ChangeDetection.Core;

public class AbstractState
{
    public AbstractStateId StateId { get; init; }
    public ConcreteStateId[] ConcreteStateIds { get; init; }
    public ModelIdentifier ModelIdentifier { get; init; }
    public bool IsInitial { get; init; }
    public int Counter { get; init; }
    public AbstractActionId[] InAbstractActions { get; init; }
    public AbstractActionId[] OutAbstractActions { get; init; }
}
