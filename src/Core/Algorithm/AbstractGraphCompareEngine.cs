using Testar.ChangeDetection.Core.Graph;
using Testar.ChangeDetection.Core.Settings;

namespace Testar.ChangeDetection.Core.Algorithm;

public interface ICompareGraph
{
    Task<CompareResults> CompareAsync(Model oldModel, Model newModel);
}

public class AbstractGraphCompareEngine : ICompareGraph
{
    private readonly IRetrieveGraphForComparison graphRetriever;
    private readonly ICompareVertices verticesComparer;
    private readonly IStartingAbstractState startingAbstractStates;
    private readonly IDetectChangeInCorrespondingStates detectChangeInCorrespondingStates;
    private readonly ComparableDataElementNameSetting comparableDataElementNameSetting;

    public AbstractGraphCompareEngine(IRetrieveGraphForComparison graphRetriever,
        ICompareVertices verticesComparer, IStartingAbstractState startingAbstractStates,
        IDetectChangeInCorrespondingStates detectChangeInCorrespondingStates, ComparableDataElementNameSetting comparableDataElementNameSetting)
    {
        this.graphRetriever = graphRetriever;
        this.verticesComparer = verticesComparer;
        this.startingAbstractStates = startingAbstractStates;
        this.detectChangeInCorrespondingStates = detectChangeInCorrespondingStates;
        this.comparableDataElementNameSetting = comparableDataElementNameSetting;
    }

    public async Task<CompareResults> CompareAsync(Model oldModel, Model newModel)
    {
        // check for similar abstract attributes,
        CheckEqualAbstractAttributes(oldModel, newModel);

        // first we retrieve the complete graph from the graph server for both models
        var oldGraphApp = await graphRetriever.RetrieveAsync(oldModel);
        var newGraphApp = await graphRetriever.RetrieveAsync(newModel);

        // determine the starting vertices
        var (oldAbstractState, newAbstractState) = startingAbstractStates.Find(oldGraphApp, newGraphApp);

        // Start checking the states
        CompareAbstractStates(oldAbstractState, newAbstractState, oldGraphApp, newGraphApp);

        return new CompareResults
        {
            OldGraphApp = oldGraphApp,
            NewGraphApp = newGraphApp,
        };
    }

    private static void CheckEqualAbstractAttributes(Model oldModel, Model newModel)
    {
        var listA_in_listB = oldModel.AbstractStates.All(a => newModel.AbstractStates.Any(b => b.Equals(a)));
        var listB_in_listA = newModel.AbstractStates.All(b => oldModel.AbstractStates.Any(a => a.Equals(b)));

        if (!(listA_in_listB && listB_in_listA))
        {
            throw new ApplicationUsesDifferentAbstractAttributes(oldModel, newModel);
        }
    }

    private void CompareAbstractStates(Vertex oldAbstractState, Vertex newAbstractState, AppGraph oldGraphApp, AppGraph newGraphApp)
    {
        // set corresponding ids for each state
        newAbstractState["CD_CorrespondingId"] = oldAbstractState["stateId"];
        oldAbstractState["CD_CorrespondingId"] = newAbstractState["stateId"];

        // mark steps as handeld since we do not want to loop around
        // the graph and handeld states twice
        oldAbstractState.IsHandled = true;
        newAbstractState.IsHandled = true;

        verticesComparer.CompareProperties(oldAbstractState, newAbstractState);
        
        // If the corresponding abstract states are different, or the abstract actions of the corresponding states do not match
        // The states contain changes
        if (detectChangeInCorrespondingStates.AreAbstractStatesDifferent(oldAbstractState, newAbstractState) ||
            detectChangeInCorrespondingStates.AreAbstractActionsDifferent(oldAbstractState, newAbstractState, oldGraphApp, newGraphApp))
        {
            newAbstractState["CD_ContainsChanges"] = new PropertyValue(bool.TrueString);
            oldAbstractState["CD_ContainsChanges"] = new PropertyValue(bool.TrueString);
        }

        var abstractActionsForOldState = oldGraphApp.FindAbstractActionsFor(oldAbstractState).ToArray();
        var unhandeldActions = newGraphApp
            .FindAbstractActionsFor(newAbstractState)
            .Where(x => !x.IsHandled)
            .ToList();

        foreach (var action in unhandeldActions)
        {
            CompareActions(action, abstractActionsForOldState, oldGraphApp, newGraphApp);
        }
    }

    private void CompareActions(Edge action, Edge[] actionsStateFromApp1, AppGraph oldGraphApp, AppGraph graphApp2)
    {
        // always mark as handeld for prevent double handling
        action.IsHandled = true;
        // actopm
        var correspondingAction = actionsStateFromApp1.FirstOrDefault(x => x[comparableDataElementNameSetting.Value] == action[comparableDataElementNameSetting.Value]);
        if (correspondingAction is null)
        {
            // this must be a new or altered action since a corresponding action
            // was not found in the abstract state.
            // not the first version lets mark it as new

            action["CD_CompareResult"] = new PropertyValue("new");
        }
        else
        {
            // Mark both new and old model actions as match
            action["CD_CompareResult"] = new PropertyValue("match");
            correspondingAction["CD_CompareResult"] = new PropertyValue("match");

            // corresponding action found. continue with outgoing state
            correspondingAction.IsHandled = true;

            var targetState = graphApp2.AbstractStates
                .First(x => x.Document.Id == action.TargetId)
                .DocumentAsVertex();

            var correspondingTargetState = oldGraphApp.AbstractStates
                .First(x => x.Document.Id == correspondingAction.TargetId)
                .DocumentAsVertex();

            // first check it handling is needed
            if (!(targetState.IsHandled || correspondingTargetState.IsHandled))
            {
                // we now have two states that are coresponding in both graph.
                // check the differences between the two states.
                CompareAbstractStates(correspondingTargetState, targetState, oldGraphApp, graphApp2);
            }
        }
    }
}