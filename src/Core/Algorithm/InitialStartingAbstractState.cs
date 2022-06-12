using Testar.ChangeDetection.Core.Graph;

namespace Testar.ChangeDetection.Core.Algorithm;

public class InitialStartingAbstractState : IStartingAbstractState
{
    public Vertex Find(AppGraph graphApp)
    {
        return graphApp.InitialAbstractState
            ?? throw new ComparisonException("Unable to find initial abstract state in app graph");
    }
}