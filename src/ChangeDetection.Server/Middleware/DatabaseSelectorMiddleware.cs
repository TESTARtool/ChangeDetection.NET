namespace Testar.ChangeDetection.Server.Middleware;

public class DatabaseSelectorMiddleware
{
    private readonly RequestDelegate next;

    public DatabaseSelectorMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public Task InvokeAsync(HttpContext context)
    {
        var values = context.Request.RouteValues;

        foreach (var value in values)
        {
            Console.WriteLine($"{value.Key} - {value.Value}");
        }

        return next.Invoke(context);
    }
}