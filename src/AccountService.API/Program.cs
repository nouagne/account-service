using Serilog;
using AccountService.API.MiddleWares;

var builder = WebApplication.CreateBuilder(args);

// Config Serilog
Serilog.Debugging.SelfLog.Enable(Console.Error);
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHealthChecks();

var app = builder.Build();

// Log l'environnement pour être sûr
app.Logger.LogInformation("ASPNETCORE_ENVIRONMENT = {env}", app.Environment.EnvironmentName);

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(o => { o.SwaggerEndpoint("/openapi/v1.json", "AccountService v1"); o.RoutePrefix = "openapi"; });
}

app.MapHealthChecks("/health");
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => Results.Redirect("/openapi"));

app.Run();