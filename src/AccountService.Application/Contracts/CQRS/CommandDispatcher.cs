using Microsoft.Extensions.DependencyInjection;

namespace AccountService.Application.Contracts.CQRS;

public class CommandDispatcher(IServiceProvider sp): ICommandDispatcher
{
    public async Task<TResult> Send<TResult>(ICommand<TResult> command, CancellationToken ct)
    {
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));
        var handler = (dynamic)sp.GetRequiredService(handlerType);
        return await handler.Handle((dynamic)command, ct);
    }
}