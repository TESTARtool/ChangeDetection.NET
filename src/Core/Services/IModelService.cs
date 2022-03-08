using System.Text.Json;

namespace Testar.ChangeDetection.Core.Services;

public interface IModelService
{
    IAsyncEnumerable<Model> AllModels();
}

public class ModelService : IModelService
{
    private readonly IChangeDetectionHttpClient client;

    public ModelService(IChangeDetectionHttpClient client)
    {
        this.client = client;
    }

    public async IAsyncEnumerable<Model> AllModels()
    {
        var applications = await new OrientDbCommand("SELECT FROM AbstractStateModel")
          .ExecuteOn<ModelJson>(client)
          .ToArrayAsync();

        foreach (var application in applications)
        {
            yield return new Model
            {
                AbstractionAttributes = application.AbstractionAttributes,
                AbstractStates = Array.Empty<AbstractState>(),
                Name = application.ApplicationName,
                Version = application.ApplicationVersion,
                ModelIdentifier = new ModelIdentifier(application.ModelIdentifier),
                TestSequences = await FetchTestSequences(application.ModelIdentifier).ToArrayAsync()
            };
        }
    }

    private static Verdict StringToVerdict(string value) => value switch
    {
        "COMPLETED_SUCCESFULLY" => Verdict.Success,
        "INTERRUPTED_BY_USER" => Verdict.InterruptByUser,
        "INTERRUPTED_BY_ERROR" => Verdict.InterruptBySystem,
        _ => Verdict.Unknown,
    };

    private async IAsyncEnumerable<TestSequence> FetchTestSequences(string modelIdentifier)
    {
        var sequences = await new OrientDbCommand("SELECT FROM TestSequence WHERE modelIdentifier = :identifier ORDER BY startDateTime ASC")
            .AddParameter("identifier", modelIdentifier)
            .ExecuteOn<SequenceJson>(client)
            .ToArrayAsync();

        foreach (var sequence in sequences)
        {
            yield return new TestSequence
            {
                Id = new SequenceId(sequence.SequenceId),
                StartDateTime = DateTime.ParseExact(sequence.StartDateTime, "yyyy-MM-dd HH:mm:ss", provider: null),
                NumberOfSteps = await NumberOfStepsAsync(sequence.SequenceId),
                NumberOfErrors = await NumberOfErrorsAsync(sequence.SequenceId),
                IsSequenceDeterministic = !await IsSequenceNonDeterministicAsync(sequence.SequenceId),
                Verdict = StringToVerdict(sequence.Verdict),
            };
        }
    }

    private async Task<int> NumberOfErrorsAsync(string sequenceId)
    {
        return await new OrientDbCommand("SELECT COUNT(*) as nr FROM(TRAVERSE out('FirstNode'), out('SequenceStep') FROM (SELECT FROM TestSequence WHERE sequenceId = :sequenceId)) WHERE @class = 'SequenceNode' AND containsErrors = true")
            .AddParameter("sequenceId", sequenceId)
            .ExecuteOn<JsonElement>(client)
            .Select(x => x.GetProperty("nr").GetInt32())
            .SingleAsync();
    }

    private async Task<int> NumberOfStepsAsync(string sequenceId)
    {
        return await new OrientDbCommand("SELECT COUNT(*) as nr FROM SequenceNode WHERE sequenceId = :sequenceId")
            .AddParameter("sequenceId", sequenceId)
            .ExecuteOn<JsonElement>(client)
            .Select(x => x.GetProperty("nr").GetInt32())
            .SingleAsync();
    }

    private async Task<bool> IsSequenceNonDeterministicAsync(string sequenceId)
    {
        return await new OrientDbCommand("SELECT FROM (TRAVERSE outE('SequenceStep') FROM (TRAVERSE out('SequenceStep') FROM (SELECT FROM (TRAVERSE out('FirstNode') FROM (select from TestSequence where sequenceId = :sequenceId)) where @class = 'SequenceNode'))) where @class = 'SequenceStep' AND nonDeterministic = true")
            .AddParameter("sequenceId", sequenceId)
            .ExecuteOn<JsonElement>(client)
            .AnyAsync();
    }

    private class SequenceJson
    {
        public string SequenceId { get; set; }
        public string StartDateTime { get; set; }
        public string Verdict { get; set; }
    }

    private class ModelJson
    {
        public string ModelIdentifier { get; init; }
        public string ApplicationVersion { get; init; }
        public string ApplicationName { get; set; }
        public string[] AbstractionAttributes { get; set; }
    }
}