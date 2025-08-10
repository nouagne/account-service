using System.Net;
using System.Text.Json;
using TemplateProject.API.Models;
using TemplateProject.Domain.Exceptions;

namespace TemplateProject.API.MiddleWares;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var traceId = context.TraceIdentifier;
            logger.LogError(ex, "Unhandled exception - TraceId: {TraceId}", traceId);

            var errorResponse = new ErrorResponse
            {
                TraceId = traceId,
                Detail = ex.Message
            };

            context.Response.ContentType = "application/json";

            switch (ex)
            {
                case NotFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.StatusCode = 404;
                    errorResponse.Title = "Resource Not Found";
                    break;
                
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.StatusCode = 500;
                    errorResponse.Title = "Internal Server Error";
                    errorResponse.Detail = "An unexpected error occurred.";
                    break;
            }

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, options));
        }
    }
}
