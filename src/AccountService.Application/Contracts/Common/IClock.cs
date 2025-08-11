namespace AccountService.Application.Contracts.Common;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}