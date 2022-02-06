using MediatR;

namespace Testar.ChangeDetection.Core.Requests;

public class AllApplicationRequest : IRequest<Application[]>
{
}

public class AllApplicationRequestHandler : IRequestHandler<AllApplicationRequest, Application[]>
{
    private readonly IOrientDbCommand orientDbCommand;

    public AllApplicationRequestHandler(IOrientDbCommand orientDbCommand)
    {
        this.orientDbCommand = orientDbCommand;
    }

    public async Task<Application[]> Handle(AllApplicationRequest request, CancellationToken cancellationToken)
    {
        var sql = "SELECT FROM AbstractStateModel";

        var applications = await orientDbCommand.ExecuteQueryAsync<ApplicationJson>(sql);

        return applications
            .Select(x => new Application
            {
                AbstractionAttributes = x.AbstractionAttributes,
                AbstractStates = Array.Empty<AbstractState>(),
                ApplicationName = x.ApplicationName,
                ApplicationVersion = x.ApplicationVersion,
                ModelIdentifier = new ModelIdentifier(x.ModelIdentifier)
            })
            .ToArray();
    }

    private class ApplicationJson
    {
        public string ModelIdentifier { get; init; }
        public string ApplicationVersion { get; init; }
        public string ApplicationName { get; set; }
        public string[] AbstractionAttributes { get; set; }
    }
}