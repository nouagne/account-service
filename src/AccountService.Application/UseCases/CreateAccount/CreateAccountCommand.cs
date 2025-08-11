using AccountService.Application.Contracts.CQRS;

namespace AccountService.Application.UseCases.CreateAccount;

public class CreateAccountCommand : ICommand<CreateAccountResult>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}