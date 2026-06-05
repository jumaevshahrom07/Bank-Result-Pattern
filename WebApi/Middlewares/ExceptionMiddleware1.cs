using System.Text.Json;
using Npgsql;

namespace WebApi.Middlewares;

public class ExceptionMiddleware1
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware1> _logger;

    public ExceptionMiddleware1(RequestDelegate next, ILogger<ExceptionMiddleware1> logger)
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

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(
                    new
                    {
                        Error = ex.Message
                    }
                )
            );
        }

        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "Database error");
            context.Response.StatusCode = 500;

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(
                    new
                    {
                        Error = "Database error"
                    }
                )
            );
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception - Необработанная ошибка");
            context.Response.StatusCode = 500;

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(
                    new
                    {
                        Error = "Internal server error"
                    }
                )
            );
        }
    }
}