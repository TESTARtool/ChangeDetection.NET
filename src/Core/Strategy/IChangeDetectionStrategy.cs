namespace Testar.ChangeDetection.Core.Strategy;

public interface IChangeDetectionStrategy
{
    Task ExecuteChangeDetectionAsync(Application control, Application test);
}