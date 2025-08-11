namespace AccountService.Application.UseCases.CreateAccount;

public sealed class CreateAccountResult(Guid accountId)
{
    public Guid AccountId { get; } = accountId;
}