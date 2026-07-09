using MacroViva.Domain.Common;

namespace MacroViva.Domain.ValueObjects;

public sealed record BodyMetrics
{
    public BodyMetrics(decimal weightKg, decimal heightCm, decimal? bodyFatPercentage = null)
    {
        WeightKg = Guard.AgainstZeroOrNegative(weightKg, nameof(weightKg));
        HeightCm = Guard.AgainstZeroOrNegative(heightCm, nameof(heightCm));

        if (bodyFatPercentage is < 0 or > 100)
        {
            throw new DomainException("bodyFatPercentage must be between 0 and 100.");
        }

        BodyFatPercentage = bodyFatPercentage;
    }

    public decimal WeightKg { get; }

    public decimal HeightCm { get; }

    public decimal? BodyFatPercentage { get; }
}
