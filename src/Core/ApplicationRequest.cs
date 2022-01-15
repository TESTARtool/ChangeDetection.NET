﻿using MediatR;

namespace Testar.ChangeDetection.Core;

public class ApplicationRequest : IRequest<Application>
{
    public string ApplicationName { get; init; }
    public string ApplicationVersion { get; init; }
}

public class ApplicationRequestHandler : IRequestHandler<ApplicationRequest, Application>
{
    private readonly IOrientDbCommand orientDbCommand;

    public ApplicationRequestHandler(IOrientDbCommand orientDbCommand)
    {
        this.orientDbCommand = orientDbCommand;
    }

    public async Task<Application> Handle(ApplicationRequest request, CancellationToken cancellationToken)
    {
        var application = await GetApplicationAsync(request);
        var sql = $"SELECT FROM AbstractState WHERE modelIdentifier = '{application.modelIdentifier}'";

        var states = await orientDbCommand.ExecuteQueryAsync<AbstractStateJson>(sql);

        return new Application
        {
            ApplicationName = request.ApplicationName,
            ApplicationVersion = request.ApplicationVersion,
            ModelIdentifier = new ModelIdentifier(application.modelIdentifier),
            AbstractionAttributes = application.abstractionAttributes,
            AbstractStates = states.Select(x => new AbstractState
            {
                ConcreteStateIds = x.concreteStateIds.Select(x => new ConcreteStateId(x)).ToArray(),
                Counter = x.counter,
                IsInitial = x.isInitial,
                ModelIdentifier = new ModelIdentifier(application.modelIdentifier),
                StateId = new AbstractStateId(x.stateId),
                InAbstractActions = x.in_AbstractAction.Select(x => new AbstractActionId(x)).ToArray(),
                OutAbstractActions = x.out_AbstractAction.Select(x => new AbstractActionId(x)).ToArray()
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
        public string[] in_AbstractAction { get; set; }
        public string[] out_AbstractAction { get; set; }
        public string stateId { get; set; }
        public string[] concreteStateIds { get; set; }
        public bool isInitial { get; set; }
        public int counter { get; set; }
    }

    private class ApplicationJson
    {
        public string modelIdentifier { get; init; }

        public string[] abstractionAttributes { get; set; }
    }
}