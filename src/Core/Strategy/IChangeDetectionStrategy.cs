namespace Testar.ChangeDetection.Core.Strategy;

public interface IChangeDetectionStrategy
{
    Task ExecuteChangeDetectionAsync(Application control, Application test, IFileOutputHandler fileOutputHandler);
}

public interface IFileOutputHandler
{
    string GetFilePath(string fileName);
}