using System.Text.Json;

namespace Testar.ChangeDetection.Core.OrientDb;

public static partial class OrientDbLogMessages
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Debug,
        Message = "OrientDb was requested with query '{sql}'. The execution plan was {executionPlan}.")]
    public static partial void LogExecutionPlan(this ILogger logger, string sql, JsonElement executionPlan);
}