using AccountService.Application.Contracts.Common;

namespace AccountService.Infrastructure;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}