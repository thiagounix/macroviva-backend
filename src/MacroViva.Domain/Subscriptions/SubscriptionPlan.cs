using MacroViva.Domain.Common;
using MacroViva.Domain.Enums;
using MacroViva.Domain.ValueObjects;

namespace MacroViva.Domain.Subscriptions;

public sealed class SubscriptionPlan : AggregateRoot
{
    private SubscriptionPlan(
        Guid id,
        LocalizedName name,
        SubscriptionTier tier,
        Money price,
        DateRange availability)
        : base(id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Tier = tier;
        Price = price ?? throw new ArgumentNullException(nameof(price));
        Availability = availability ?? throw new ArgumentNullException(nameof(availability));
    }

    public LocalizedName Name { get; private set; }

    public SubscriptionTier Tier { get; }

    public Money Price { get; private set; }

    public DateRange Availability { get; private set; }

    public static SubscriptionPlan Create(
        Guid id,
        LocalizedName name,
        SubscriptionTier tier,
        Money price,
        DateRange availability)
    {
        return new SubscriptionPlan(id, name, tier, price, availability);
    }

    public bool IsAvailableOn(DateOnly date)
    {
        return Availability.Contains(date);
    }
}
