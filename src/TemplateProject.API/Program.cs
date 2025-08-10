using Serilog;
using TemplateProject.API.MiddleWares;

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

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My API V1");
        options.RoutePrefix = "OpenApi";
    });
}

app.MapHealthChecks("/health");
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "Hello world!");

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("✅ Test log manuel écrit via Program.cs");

app.Run();