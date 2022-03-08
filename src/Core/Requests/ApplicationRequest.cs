using MediatR;

namespace Testar.ChangeDetection.Core.Requests;

public class ApplicationRequest : IRequest<Model>
{
    public string ApplicationName { get; init; }
    public string ApplicationVersion { get; init; }
}

public class ApplicationRequestHandler : IRequestHandler<ApplicationRequest, Model>
{
    private readonly IChangeDetectionHttpClient client;

    public ApplicationRequestHandler(IChangeDetectionHttpClient client)
    {
        this.client = client;
    }

    public async Task<Model> Handle(ApplicationRequest request, CancellationToken cancellationToken)
    {
        var application = await GetApplicationAsync(request);
        var states = new OrientDbCommand("SELECT FROM AbstractState WHERE modelIdentifier = :modelIdentifier")
            .AddParameter("modelIdentifier", application.ModelIdentifier)
            .ExecuteOn<AbstractStateJson>(client)
            .Select(x => new AbstractState
            {
                ConcreteStateIds = x.ConcreteStateIds.Select(x => new ConcreteStateId(x)).ToArray(),
                Counter = x.Counter,
                IsInitial = x.IsInitial,
                ModelIdentifier = new ModelIdentifier(application.ModelIdentifier),
                StateId = new AbstractStateId(x.StateId),
                InAbstractActions = x.In_AbstractAction.Select(x => new OrientDbId(x)).ToArray(),
                OutAbstractActions = x.Out_AbstractAction.Select(x => new OrientDbId(x)).ToArray()
            })
            .ToArrayAsync();

        return new Model
        {
            Name = request.ApplicationName,
            Version = request.ApplicationVersion,
            ModelIdentifier = new ModelIdentifier(application.ModelIdentifier),
            AbstractionAttributes = application.AbstractionAttributes,
            AbstractStates = await states,
            TestSequences = Array.Empty<TestSequence>()
        };
    }

    private async Task<ApplicationJson> GetApplicationAsync(ApplicationRequest request)
    {
        var application = await new OrientDbCommand("SELECT FROM AbstractStateModel WHERE applicationName = :applicationName AND applicationVersion = :applicationVersion")
            .AddParameter("applicationName", request.ApplicationName)
            .AddParameter("applicationVersion", request.ApplicationVersion)
            .ExecuteOn<ApplicationJson>(client)
            .SingleOrDefaultAsync();

        return application ?? throw new Exception($"Cannot find application '{request.ApplicationName}' with version '{request.ApplicationVersion}'");
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