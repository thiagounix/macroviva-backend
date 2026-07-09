using MacroViva.Application.Abstractions.Services;

namespace MacroViva.Infrastructure.Services;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

    public DateOnly Today => DateOnly.FromDateTime(UtcNow.UtcDateTime);
}
