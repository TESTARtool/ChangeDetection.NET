using Testar.ChangeDetection.Core.Graph;

namespace Testar.ChangeDetection.Core.Algorithm;

public class InitialStartingAbstractState : IStartingAbstractState
{
    public (Vertex abstractState1, Vertex abstractState2) DetemineStartingStates(AppGraph graphApp1, AppGraph graphApp2)
    {
        var initalStateApp1 = graphApp1.InitialAbstractState;
        if (initalStateApp1 is null)
        {
            throw new ComparisonException("Unable to find initial abstract state in graph app 1");
        }

        var initalStateApp2 = graphApp2.InitialAbstractState;
        if (initalStateApp2 is null)
        {
            throw new ComparisonException("Unable to find initial abstract state in graph app 2");
        }

        return (initalStateApp1, initalStateApp2);
    }
}