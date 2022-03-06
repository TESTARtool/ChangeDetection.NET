using System.Text.Json;

namespace Testar.ChangeDetection.Core.OrientDb;

public static partial class OrientDbLogMessages
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Debug,
        Message = "OrientDb was requested with query '{sql}'. The execution plan was {executionPlan}.")]
    public static partial void LogExecutionPlan(this ILogger logger, string sql, JsonElement executionPlan);

    [LoggerMessage(EventId = 1, Level = LogLevel.Information,
        Message = "OrientDb server is disconnected for url '{url}'")]
    public static partial void LogDisconnect(this ILogger logger, Uri url);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information,
        Message = "OrientDb '{url}' login succesfull on database '{databaseName}'")]
    public static partial void LogSuccesfull(this ILogger logger, Uri url, string databaseName);

    [LoggerMessage(EventId = 3, Level = LogLevel.Debug,
        Message = "OrientDb cookie is '{cookie}'")]
    public static partial void LogOrientDbCookie(this ILogger logger, string cookie);
}