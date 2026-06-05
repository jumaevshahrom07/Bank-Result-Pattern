using System.Text.Json;
using Npgsql;

namespace WebApi.Middlewares;

public class ExceptionMiddleware2
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware2> _logger;

    public ExceptionMiddleware2(RequestDelegate next, ILogger<ExceptionMiddleware2> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }

        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex.Message);
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";

            var response = new
            {
                Error = ex.Message,
                Statuscode = 400
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }

        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "Database error");
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var response = new
            {
                Error = "Database error",
                Statuscode = 500
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception - Необработанная ошибка");
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var response = new
            {
                Error = "Internal server error",
                Statuscode = 500
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}