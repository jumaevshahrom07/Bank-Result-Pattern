using System.Diagnostics;

namespace WebApi.Middlewares;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation($"Request:metod, path {context.Request.Method} {context.Request.Path}");

        await _next(context);
        stopwatch.Stop();
        _logger.LogInformation($"Response: {context.Response.StatusCode} processed in {stopwatch.ElapsedMilliseconds} ms");

        if (stopwatch.ElapsedMilliseconds > 3000)
        {
            _logger.LogInformation($"Slow request: metod, path {context.Request.Method} {context.Request.Path} {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}