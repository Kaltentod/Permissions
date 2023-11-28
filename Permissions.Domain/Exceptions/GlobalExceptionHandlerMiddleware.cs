using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Permissions.Domain.Exceptions;
using System.Net;
using System.Text.Json;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            string message;
            string traceError;

            switch (ex)
            {
                case PermissionNotFoundException permissionNotFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    message = ex.Message;
                    traceError = permissionNotFoundException.TraceError;
                    break;
                case PermissionTypeNotFoundException permissionTypeNotFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    message = ex.Message;
                    traceError = permissionTypeNotFoundException.TraceError;
                    break;
                default:
                    _logger.LogError($"Unhandled exception: {ex}");
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    traceError = "Internal Server Error: Something went wrong while processing your request";
                    message = "Comunique el error al equipo de Desarrollo de Sistemas";
                    break;
            }

            var result = JsonSerializer.Serialize(new { error = traceError, details = message });
            await context.Response.WriteAsync(result);
        }
    }
}