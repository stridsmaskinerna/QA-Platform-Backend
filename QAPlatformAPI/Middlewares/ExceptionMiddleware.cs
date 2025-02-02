using System.Data.Common;
using System.Text.Json;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace QAPlatformAPI.Middlewares;

public class ExceptionMiddleware(
    RequestDelegate next,
    ProblemDetailsFactory problemDetailsFactory,
    JsonSerializerOptions jsonOptions
)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ApiException ex)
        {
            await HandleExceptionAsync(context, ex);
        }
        catch (DbException)
        {
            var apiException = new InternalServerException();
            await HandleExceptionAsync(context, apiException);
        }
        catch (Exception)
        {
            var apiException = new InternalServerException();
            await HandleExceptionAsync(context, apiException);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        ApiException exception
    )
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = exception.StatusCode;

        var problemDetails = problemDetailsFactory.CreateProblemDetails(
            context,
            exception.StatusCode,
            exception.Title,
            detail: exception.Detail,
            instance: context.Request.Path
        );

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, jsonOptions));
    }
}
