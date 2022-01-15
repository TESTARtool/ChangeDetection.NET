using Microsoft.Extensions.Logging;

namespace Testar.ChangeDetection.ConsoleApp;

public static partial class LogMessageDefinitionGen
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = "Application has started")]
    public static partial void LogApplicationStarted(this ILogger logger);
}