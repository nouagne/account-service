﻿namespace AccountService.Application.Common;

public interface IClock
{
    DateTime UtcNow { get; }
}