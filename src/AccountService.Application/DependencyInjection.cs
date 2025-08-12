using AccountService.Application.Contracts.CQRS;
using AccountService.Application.UseCases.CreateAccount;
using AccountService.Application.UseCases.GetAccount;
using Microsoft.Extensions.DependencyInjection;

namespace AccountService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddTransient<ICommandHandler<CreateAccountCommand, CreateAccountResult>, CreateAccountHandler>();
        services.AddTransient<ICommandHandler<GetAccountCommand, GetAccountResult>, GetAccountHandler>();

        return services;
    }
}