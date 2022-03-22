using System.Text.Json;

namespace Testar.ChangeDetection.Core.Services;

public class TestSequenceVisualisation
{
    public int Order { get; set; }
    public string BeforeImage { get; set; }
    public string AfterImage { get; set; }
    public string ActionDescription { get; set; }
    public bool IsDeterministic { get; set; }
}

public interface IModelService
{
    IAsyncEnumerable<Model> AllModels();

    IAsyncEnumerable<TestSequence> TestSequences(ModelIdentifier modelIdentifier);

    IAsyncEnumerable<TestSequenceVisualisation> GetTestSequenceActions(SequenceId testSequenceId);
}

public class ModelService : IModelService
{
    private readonly IChangeDetectionHttpClient client;

    public ModelService(IChangeDetectionHttpClient client)
    {
        this.client = client;
    }

    public async IAsyncEnumerable<TestSequence> TestSequences(ModelIdentifier modelIdentifier)
    {
        var sequences = await new OrientDbCommand("SELECT FROM TestSequence WHERE modelIdentifier = :identifier ORDER BY startDateTime ASC")
           .AddParameter("identifier", modelIdentifier.Value)
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
            };
        }
    }

    public async IAsyncEnumerable<TestSequenceVisualisation> GetTestSequenceActions(SequenceId testSequenceId)
    {
        var sequenceNodes = await new OrientDbCommand("SELECT FROM sequenceNode WHERE sequenceId = :sequenceId")
            .AddParameter("sequenceId", testSequenceId.Value)
            .ExecuteOn<SequenceNodeJson>(client)
            .ToArrayAsync();

        var sequenceSteps = await GetSequenceSteps(sequenceNodes)
            .ToArrayAsync();

        // transform the sequence node into a test visualisation
        // a test visualisation consist of 2 images (from the concrete states)
        // and a label containing the desciption concrete action.
        // the concrete action description is included in the sequence step

        // we will start with the firstNode and work our way from there
        // until we find a step without a next step (== out_sequenceStep empty or null)
        var firstNode = sequenceNodes.FirstOrDefault(x => x.In_FirstNode.Any());
        if (firstNode is null)
        {
            // if first node is not found no need to continue
            // technical out_sequenceStep could be null or empty but lets handle that later on
            yield break;
        }

        var order = 0;
        var selectedNode = firstNode;
        var beforeImage = await GetImageFromSequenceNode(selectedNode);
        do
        {
            var step = sequenceSteps.FirstOrDefault(x => x.Id == selectedNode.Out_SequenceStep.FirstOrDefault());
            selectedNode = sequenceNodes.FirstOrDefault(x => x.SequenceId == step?.In);
            var afterImage = await GetImageFromSequenceNode(selectedNode);

            yield return new TestSequenceVisualisation
            {
                Order = order++,
                BeforeImage = beforeImage,
                ActionDescription = step?.ActionDescription ?? string.Empty,
                AfterImage = afterImage,
                IsDeterministic = !(step?.NonDeterministic ?? false)
            };

            // the after image is the before image for a next test sequence visualisation
            // lets re-use it to prevent extra database calls
            beforeImage = afterImage;
        } while (selectedNode is not null && selectedNode.Out_SequenceStep.Any());
    }

    private static Verdict StringToVerdict(string value) => value switch
    {
        "COMPLETED_SUCCESFULLY" => Verdict.Success,
        "INTERRUPTED_BY_USER" => Verdict.InterruptByUser,
        "INTERRUPTED_BY_ERROR" => Verdict.InterruptBySystem,
        _ => Verdict.Unknown,
    };

    private async Task<string> GetImageFromSequenceNode(SequenceNodeJson? sequenceNode)
    {
        if (sequenceNode is null)
        {
            return string.Empty;
        }

        var state = await new OrientDbCommand("SELECT screenshot FROM concreteState WHERE ConcreteIDCustom = :concreteId")
              .AddParameter("concreteId", sequenceNode.ConcreteStateId)
              .ExecuteOn<ScreenshotJson>(client)
              .FirstAsync();

        return await client.DocumentAsBase64Async(new OrientDbId(state.Screenshot)) ?? string.Empty;
    }

    private async IAsyncEnumerable<SequenceStep> GetSequenceSteps(SequenceNodeJson[] actions)
    {
        var nodes = actions.Where(x => x.Out_SequenceStep.Any()).ToList();
        foreach (var action in nodes)
        {
            yield return await new OrientDbCommand("SELECT FROM SequenceStep WHERE @rid = :id")
                .AddParameter("id", action.Out_SequenceStep[0])
                .ExecuteOn<SequenceStep>(client)
                .SingleAsync();
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

    public class SequenceNodeJson
    {
        [JsonPropertyName("@rid")]
        public string SequenceId { get; set; }

        public string[] Out_SequenceStep { get; set; } = Array.Empty<string>();
        public string[] In_FirstNode { get; set; } = Array.Empty<string>();
        public string ConcreteStateId { get; set; }
    }

    private class ScreenshotJson
    {
        public string Screenshot { get; set; }
    }

    private class SequenceStep
    {
        [JsonPropertyName("@rid")]
        public string Id { get; set; }

        public string ActionDescription { get; set; }
        public string Out { get; set; }
        public string In { get; set; }
        public string concreteActionId { get; set; }
        public bool NonDeterministic { get; set; }
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
        public string[] AbstractionAttributes { get; set; } = Array.Empty<string>();
    }
}