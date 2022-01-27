using MediatR;

namespace Testar.ChangeDetection.Core.Requests;

public class ApplicationActionRequest : IRequest<ApplicationAction[]>
{
    public AbstractActionId AbstractActionId { get; set; }
}

public class ActionRequestHandler : IRequestHandler<ApplicationActionRequest, ApplicationAction[]>
{
    private readonly IOrientDbCommand dbCommand;

    public ActionRequestHandler(IOrientDbCommand dbCommand)
    {
        this.dbCommand = dbCommand;
    }

    public async Task<ApplicationAction[]> Handle(ApplicationActionRequest request, CancellationToken cancellationToken)
    {
        var action = await AbstractAction(request);

        var actions = new List<ApplicationAction>();

        foreach (var id in action.ConcreteActionIds)
        {
            var sql = $"SELECT `Desc`, actionId FROM ConcreteAction WHERE actionId = '{id}'";
            var concreteActions = (await dbCommand.ExecuteQueryAsync<ConcreteActionJson>(sql))
                .Select(x => new ApplicationAction
                {
                    AbstractActionId = request.AbstractActionId,
                    ConcreteActionId = new ConcreteActionId(id),
                    Description = x.Desc,
                })
                .ToList();

            actions.AddRange(concreteActions);
        }

        return actions.ToArray();
    }

    public async Task<ActionJson> AbstractAction(ApplicationActionRequest request)
    {
        var actions = await dbCommand.ExecuteQueryAsync<ActionJson>($"SELECT FROM AbstractAction WHERE @rid = '{request.AbstractActionId.Value.Replace("#", "").Trim()}'");

        return actions.FirstOrDefault()
            ?? throw new Exception($"Unable to find actions with Id '{request.AbstractActionId.Value}'");
    }

    public class ActionJson
    {
        public string[] ConcreteActionIds { get; set; }
    }

    public class ConcreteActionJson
    {
        public string Desc { get; set; }
    }
}