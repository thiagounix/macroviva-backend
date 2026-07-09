using MacroViva.Domain.Common;

namespace MacroViva.Domain.ValueObjects;

public sealed record DateRange
{
    public DateRange(DateOnly startsOn, DateOnly? endsOn = null)
    {
        if (endsOn.HasValue && endsOn.Value < startsOn)
        {
            throw new DomainException("Date range end cannot be before start.");
        }

        StartsOn = startsOn;
        EndsOn = endsOn;
    }

    public DateOnly StartsOn { get; }

    public DateOnly? EndsOn { get; }

    public bool Contains(DateOnly date)
    {
        return date >= StartsOn && (!EndsOn.HasValue || date <= EndsOn.Value);
    }
}
