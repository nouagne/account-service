using AccountService.Application.Contracts.CQRS;

namespace AccountService.Application.UseCases.GetAccount;

public class GetAccountCommand : ICommand<GetAccountResult>
{
    public Guid AccountId { get; init; }
}