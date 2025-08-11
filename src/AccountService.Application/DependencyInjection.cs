using AccountService.Application.Contracts.CQRS;
using AccountService.Application.UseCases.CreateAccount;
using Microsoft.Extensions.DependencyInjection;

namespace AccountService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddTransient<ICommandHandler<CreateAccountCommand, CreateAccountResult>, CreateAccountHandler>();

        return services;
    }
}