using Testar.ChangeDetection.Core.Graph;

namespace Testar.ChangeDetection.Core.Algorithm;

public interface IDetectChangeInCorrespondingStates
{
    bool ContainsChanges(Vertex oldAbstractState, Vertex newAbstractState, AppGraph oldGraphApp, AppGraph newGraphApp);
}

public class ContainsChangesWhenActionsDoNotMatch : IDetectChangeInCorrespondingStates
{
    public bool ContainsChanges(Vertex oldAbstractState, Vertex newAbstractState, AppGraph oldGraphApp, AppGraph newGraphApp)
    {
        var abstractActionsForOldState = oldGraphApp.FindAbstractActionsFor(oldAbstractState)
            .Select(x => x["actionId"].Value)
            .ToList();

        var abstractActionsForNewState = newGraphApp.FindAbstractActionsFor(newAbstractState)
            .Select(x => x["actionId"].Value)
            .ToList();

        var listA_in_listB = abstractActionsForOldState.All(a => abstractActionsForNewState.Any(b => b.Equals(a)));
        var listB_in_listA = abstractActionsForNewState.All(b => abstractActionsForOldState.Any(a => a.Equals(b)));

        return !(listA_in_listB && listB_in_listA);
    }
}