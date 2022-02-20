﻿using MediatR;

namespace Testar.ChangeDetection.Core.Requests;

public class ApplicationRequest : IRequest<Application>
{
    public string ApplicationName { get; init; }
    public string ApplicationVersion { get; init; }
}

public class ApplicationRequestHandler : IRequestHandler<ApplicationRequest, Application>
{
    private readonly IOrientDbCommandExecuter orientDbCommand;

    public ApplicationRequestHandler(IOrientDbCommandExecuter orientDbCommand)
    {
        this.orientDbCommand = orientDbCommand;
    }

    public async Task<Application> Handle(ApplicationRequest request, CancellationToken cancellationToken)
    {
        var application = await GetApplicationAsync(request);
        var sql = $"SELECT FROM AbstractState WHERE modelIdentifier = '{application.ModelIdentifier}'";

        var states = await orientDbCommand.ExecuteQueryAsync<AbstractStateJson>(sql);

        return new Application
        {
            ApplicationName = request.ApplicationName,
            ApplicationVersion = request.ApplicationVersion,
            ModelIdentifier = new ModelIdentifier(application.ModelIdentifier),
            AbstractionAttributes = application.AbstractionAttributes,
            AbstractStates = states.Select(x => new AbstractState
            {
                ConcreteStateIds = x.ConcreteStateIds.Select(x => new ConcreteStateId(x)).ToArray(),
                Counter = x.Counter,
                IsInitial = x.IsInitial,
                ModelIdentifier = new ModelIdentifier(application.ModelIdentifier),
                StateId = new AbstractStateId(x.StateId),
                InAbstractActions = x.In_AbstractAction.Select(x => new OrientDbId(x)).ToArray(),
                OutAbstractActions = x.Out_AbstractAction.Select(x => new OrientDbId(x)).ToArray()
            }).ToArray()
        };
    }

    private async Task<ApplicationJson> GetApplicationAsync(ApplicationRequest request)
    {
        var sql = "SELECT FROM AbstractStateModel WHERE " +
        $"applicationName = '{request.ApplicationName}' AND " +
        $"applicationVersion = '{request.ApplicationVersion}'";

        var entities = await orientDbCommand.ExecuteQueryAsync<ApplicationJson>(sql);

        return entities.SingleOrDefault()
            ?? throw new Exception($"Cannot find application '{request.ApplicationName}' with version '{request.ApplicationVersion}'");
    }

    private class AbstractStateJson
    {
        public string[] In_AbstractAction { get; set; }
        public string[] Out_AbstractAction { get; set; }
        public string StateId { get; set; }
        public string[] ConcreteStateIds { get; set; }
        public bool IsInitial { get; set; }
        public int Counter { get; set; }
    }

    private class ApplicationJson
    {
        public string ModelIdentifier { get; init; }

        public string[] AbstractionAttributes { get; set; }
    }
}