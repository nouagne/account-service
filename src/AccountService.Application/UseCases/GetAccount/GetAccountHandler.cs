using AccountService.Application.Contracts.CQRS;
using AccountService.Application.Contracts.Repositories;
using AccountService.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace AccountService.Application.UseCases.GetAccount;

public class GetAccountHandler(ILogger<GetAccountHandler> logger, IAccountRepository repo) : ICommandHandler<GetAccountCommand, GetAccountResult>
{
    public async Task<GetAccountResult> Handle(GetAccountCommand command, CancellationToken ct)
    {
        //validation etc
        
        var account = await repo.GetByIdAsync(command.AccountId, ct);
        if (account is null)
            throw new NotFoundException("Account", command.AccountId);

        return new GetAccountResult
        {
            AccountId = account.AccountId,
            FirstName = account.Firstname,
            LastName  = account.Lastname,
            Email     = account.Email,
            Timezone  = account.Timezone,
            CreatedAt = account.CreatedAt,
            UpdatedAt = account.UpdatedAt
        };
    }
}