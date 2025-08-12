namespace AccountService.API.Features.Accounts.Get;

public class GetAccountResponse
{
    public required Guid AccountId { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public required string Email { get; init; }
    public string? Timezone { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
}