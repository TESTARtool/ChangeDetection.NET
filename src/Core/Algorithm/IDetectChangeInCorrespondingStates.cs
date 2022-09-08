using Testar.ChangeDetection.Core.Graph;

namespace Testar.ChangeDetection.Core.Algorithm;

public interface IDetectChangeInCorrespondingStates
{
    bool ContainsChanges(Vertex oldState, Vertex newState);
}

public class ContainsChangesWhenActionsDoNotMatch : IDetectChangeInCorrespondingStates
{
    public bool ContainsChanges(Vertex oldState, Vertex newState)
    {
        return true;
    }
}