namespace Testar.ChangeDetection.Core.Strategy;

public interface IChangeDetectionStrategy
{
    string Name { get; }

    Task ExecuteChangeDetectionAsync(Application control, Application test, IFileOutputHandler fileOutputHandler);
}