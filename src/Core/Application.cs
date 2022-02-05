namespace Testar.ChangeDetection.Core;

public class Application
{
    public string[] AbstractionAttributes { get; init; }
    public string ApplicationName { get; init; }
    public string ApplicationVersion { get; init; }
    public ModelIdentifier ModelIdentifier { get; init; }
    public AbstractState[] AbstractStates { get; init; }
}
