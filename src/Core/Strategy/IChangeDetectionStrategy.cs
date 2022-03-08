namespace Testar.ChangeDetection.Core.Strategy;

public interface IChangeDetectionStrategy
{
    string Name { get; }

    Task ExecuteChangeDetectionAsync(Model control, Model test, IFileOutputHandler fileOutputHandler);
}