using MediatR;
using System.Diagnostics;
using Testar.ChangeDetection.Core.Requests;

namespace Testar.ChangeDetection.Core.Strategy.AbstractStateComparison;

public interface IFindStateDifferences
{
    IAsyncEnumerable<DeltaState> FindAddedState(Model control, Model test);

    IAsyncEnumerable<DeltaState> FindRemovedState(Model control, Model test);
}

public class FindStateDifferences : IFindStateDifferences
{
    private readonly IMediator mediator;

    public FindStateDifferences(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public async IAsyncEnumerable<DeltaState> FindAddedState(Model control, Model test)
    {
        var addedStates = test.AbstractStates
           .Where(x => !control.AbstractStates.Select(y => y.StateId).Contains(x.StateId))
           .ToList();

        foreach (var addedState in addedStates)
        {
            var concreteState = await GetConcreteStateEntities(addedState).ToListAsync();
            var outgoingDeltaActions = await GetDeltaAction(addedState.OutAbstractActions, ActionType.Outgoing);
            var incommingDeltaActions = await GetDeltaAction(addedState.InAbstractActions, ActionType.Incomming);

            yield return new DeltaState
            {
                StateId = addedState.StateId,
                ConcreteStates = concreteState,
                IncommingDeltaActions = incommingDeltaActions,
                OutgoingDeltaActions = outgoingDeltaActions,
            };
        }
    }

    public async IAsyncEnumerable<DeltaState> FindRemovedState(Model control, Model test)
    {
        var removeStates = control.AbstractStates
          .Where(x => !test.AbstractStates.Select(y => y.StateId).Contains(x.StateId))
          .ToList();

        foreach (var removedState in removeStates)
        {
            var concreteState = await GetConcreteStateEntities(removedState).ToListAsync();
            var outgoingDeltaActions = await GetDeltaAction(removedState.OutAbstractActions, ActionType.Outgoing);
            var incommingDeltaActions = await GetDeltaAction(removedState.InAbstractActions, ActionType.Incomming);

            yield return new DeltaState
            {
                StateId = removedState.StateId,
                ConcreteStates = concreteState,
                IncommingDeltaActions = incommingDeltaActions,
                OutgoingDeltaActions = outgoingDeltaActions,
            };
        }
    }

    private async Task<List<DeltaAction>> GetDeltaAction(OrientDbId[] actionIds, ActionType actionType)
    {
        var returns = new List<DeltaAction>();

        // for the delta action we need to retrieve the Description from a ConcreteAction
        // Every AbstractAction must have a corresponding concrete action

        // we have the id of the action -> find the abstract action

        foreach (var id in actionIds)
        {
            var request = new ApplicationActionRequest { AbstractActionId = id };
            var actions = (await mediator.Send(request))
                .Select(x => new DeltaAction
                {
                    ActionId = x.AbstractActionId,
                    Description = x.Description,
                    ActionType = actionType,
                }).ToList();

            returns.AddRange(actions);
        }

        return returns;
    }

    private async IAsyncEnumerable<ConcreteState> GetConcreteStateEntities(AbstractState abstractState)
    {
        foreach (var id in abstractState.ConcreteStateIds)
        {
            var concreteState = await mediator.Send(new ConcreteStateEntityRequest { ConcreteStateId = id });
            if (concreteState is not null)
            {
                yield return concreteState;
            }
        }
    }
}

public class DeltaState
{
    public AbstractStateId StateId { get; set; }
    public IEnumerable<ConcreteState> ConcreteStates { get; set; }
    public List<DeltaAction> OutgoingDeltaActions { get; set; }
    public List<DeltaAction> IncommingDeltaActions { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is DeltaState state &&
               EqualityComparer<AbstractStateId>.Default.Equals(StateId, state.StateId);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StateId);
    }
}

[DebuggerDisplay("value: {ActionId} - {Description}")]
public class DeltaAction
{
    public AbstractActionId ActionId { get; set; }
    public string Description { get; set; }
    public ActionType ActionType { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is DeltaAction action &&
               EqualityComparer<string>.Default.Equals(ActionId.Value, action.ActionId.Value);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ActionId.Value);
    }
}