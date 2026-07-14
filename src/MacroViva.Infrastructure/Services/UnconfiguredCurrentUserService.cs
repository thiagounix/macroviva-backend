using MacroViva.Application.Abstractions.Services;

namespace MacroViva.Infrastructure.Services;

public sealed class UnconfiguredCurrentUserService : ICurrentUserService
{
    public Guid UserId => throw new InvalidOperationException("No current user service is configured for this environment.");

    public bool IsAuthenticated => false;
}
