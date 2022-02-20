using MediatR;

namespace Testar.ChangeDetection.Core.Requests;

public class ApplicationActionRequest : IRequest<AbstractAction[]>
{
    public OrientDbId AbstractActionId { get; set; }
}

public class ActionRequestHandler : IRequestHandler<ApplicationActionRequest, AbstractAction[]>
{
    private readonly IOrientDbCommandExecuter dbCommand;

    public ActionRequestHandler(IOrientDbCommandExecuter dbCommand)
    {
        this.dbCommand = dbCommand;
    }

    public async Task<AbstractAction[]> Handle(ApplicationActionRequest request, CancellationToken cancellationToken)
    {
        var action = await AbstractAction(request);

        var actions = new List<AbstractAction>();

        foreach (var concreteActionId in action.ConcreteActionIds)
        {
            var sql = $"SELECT `Desc` FROM ConcreteAction WHERE actionId = '{concreteActionId}'";
            var concreteActions = (await dbCommand.ExecuteQueryAsync<ConcreteActionJson>(sql))
                .Select(x => new AbstractAction
                {
                    AbstractActionId = new AbstractActionId(action.ActionId),
                    ConcreteActionId = new ConcreteActionId(concreteActionId),
                    Description = x.Desc,
                })
                .ToList();

            actions.AddRange(concreteActions);
        }

        return actions.ToArray();
    }

    public async Task<ActionJson> AbstractAction(ApplicationActionRequest request)
    {
        var actions = await dbCommand.ExecuteQueryAsync<ActionJson>($"SELECT FROM AbstractAction WHERE @rid = '{request.AbstractActionId.Id.Replace("#", "").Trim()}'");

        return actions.SingleOrDefault()
            ?? throw new Exception($"Unable to find actions with Id '{request.AbstractActionId.Id}'");
    }

    public class ActionJson
    {
        public string ActionId { get; set; }
        public string[] ConcreteActionIds { get; set; }
    }

    public class ConcreteActionJson
    {
        public string Desc { get; set; }
    }
}