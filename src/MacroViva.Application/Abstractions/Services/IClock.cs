namespace MacroViva.Application.Abstractions.Services;

public interface IClock
{
    DateTimeOffset UtcNow { get; }

    DateOnly Today { get; }
}
