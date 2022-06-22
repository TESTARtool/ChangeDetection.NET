using Testar.ChangeDetection.Core.Graph;

namespace Testar.ChangeDetection.Core.Algorithm;

public class InitialStartingAbstractState : IStartingAbstractState
{
    public (Vertex oldAbstractState, Vertex newAbstractState) Find(AppGraph oldGraphApp, AppGraph newGraphApp)
    {
        var oldAbstractState = oldGraphApp.InitialAbstractState
            ?? throw new ComparisonException("Unable to find initial abstract state in app graph");

        var newAbstractState = newGraphApp.InitialAbstractState
            ?? throw new ComparisonException("Unable to find initial abstract state in app graph");

        return (oldAbstractState, newAbstractState);
    }
}