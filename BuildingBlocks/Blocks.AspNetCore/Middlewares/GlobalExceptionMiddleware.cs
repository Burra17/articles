using Blocks.Domain;
using Blocks.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.IIS;
using FluentValidation;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Blocks.AspNetCore.Middlewares;

public sealed class GlobalExceptionMiddleware(RequestDelegate _next, ILogger<GlobalExceptionMiddleware> _logger)
{
    private static HttpStatusCode MapStatusCode(Exception ex) => ex switch
    {
        ValidationException => HttpStatusCode.BadRequest,
        ArgumentException => HttpStatusCode.BadRequest,
        BadRequestException => HttpStatusCode.BadRequest,
        NotFoundException => HttpStatusCode.NotFound,
        DomainException => HttpStatusCode.BadRequest,
        _ => HttpStatusCode.InternalServerError,
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch(ValidationException ex)
        {
            await HandleValidationExceptionAsync(context, ex, HttpStatusCode.BadRequest);
        }
        catch(OperationCanceledException)
        {
            if (!context.Response.HasStarted)
                context.Response.StatusCode = 499;
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = MapStatusCode(exception);

        if(statusCode == HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Unhandled exeption. TraceId={TraceId}", context.TraceIdentifier);
        else
            _logger.LogDebug(exception, exception.Message, context.TraceIdentifier);

        context.Response.StatusCode = (int) statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            context.Response.StatusCode,
            exception.Message,
            Details = exception.StackTrace
        };
        var responseJson = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(responseJson);
    }

    private static Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception, HttpStatusCode statusCode)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        var validationErrors = exception.Errors.Select(e => new
        {
            e.PropertyName,
            e.ErrorMessage
        });

        var response = new
        {
            context.Response.StatusCode,
            exception.Message,
            Details = exception.StackTrace,
            Errors = validationErrors
        };
        var responseJson = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(responseJson);
    }
}
