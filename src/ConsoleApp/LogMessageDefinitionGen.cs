namespace Testar.ChangeDetection.ConsoleApp;

public static partial class LogMessageDefinitionGen
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "Application has started")]
    public static partial void LogApplicationStarted(this ILogger logger);
}

public static partial class LogMessageDefinitionGen2
{
    [LoggerMessage(EventId = 2, Level = LogLevel.Debug, Message = "Starting with arguments: {args}")]
    public static partial void LogApplicationArguments(this ILogger logging, string args);
}