namespace AccountService.API.MiddleWares;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        logger.LogInformation("Incoming request: {method} {url}",
            context.Request.Method, context.Request.Path);

        await next(context); // ↩ Passe au middleware suivant (ou au controller)
    }
}
