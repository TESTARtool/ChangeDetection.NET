namespace Testar.ChangeDetection.Core;

public class Application
{
    public string[] AbstractionAttributes { get; init; }
    public string ApplicationName { get; init; }
    public string ApplicationVersion { get; init; }
    public ModelIdentifier ModelIdentifier { get; init; }
    public AbstractState[] AbstractStates { get; init; }
    public TestSequence[] TestSequences { get; init; }
}

public class TestSequence
{
    public int NumberOfSteps { get; init; }
    public int NumberOfErrors { get; init; }
    public bool IsSequenceDeterministic { get; init; }
    public SequenceId Id { get; init; }
    public Verdict Verdict { get; init; }
    public DateTime StartDateTime { get; internal set; }
}

public enum Verdict
{
    Success,
    InterruptByUser,
    InterruptBySystem,
    Unknown,
}