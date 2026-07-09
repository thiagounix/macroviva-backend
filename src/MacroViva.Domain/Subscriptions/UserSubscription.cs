using MacroViva.Domain.Common;
using MacroViva.Domain.Enums;
using MacroViva.Domain.ValueObjects;

namespace MacroViva.Domain.Subscriptions;

public sealed class UserSubscription : AggregateRoot
{
    private UserSubscription()
    {
        PriceSnapshot = null!;
        ActivePeriod = null!;
    }

    private UserSubscription(
        Guid id,
        Guid userId,
        Guid subscriptionPlanId,
        SubscriptionTier tierSnapshot,
        Money priceSnapshot,
        DateRange activePeriod)
        : base(id)
    {
        UserId = Guard.AgainstEmpty(userId, nameof(userId));
        SubscriptionPlanId = Guard.AgainstEmpty(subscriptionPlanId, nameof(subscriptionPlanId));
        TierSnapshot = tierSnapshot;
        PriceSnapshot = priceSnapshot ?? throw new ArgumentNullException(nameof(priceSnapshot));
        ActivePeriod = activePeriod ?? throw new ArgumentNullException(nameof(activePeriod));
    }

    public Guid UserId { get; }

    public Guid SubscriptionPlanId { get; }

    public SubscriptionTier TierSnapshot { get; }

    public Money PriceSnapshot { get; }

    public DateRange ActivePeriod { get; }

    public static UserSubscription Start(
        Guid id,
        Guid userId,
        SubscriptionPlan plan,
        DateRange activePeriod)
    {
        ArgumentNullException.ThrowIfNull(plan);

        return new UserSubscription(
            id,
            userId,
            plan.Id,
            plan.Tier,
            plan.Price,
            activePeriod);
    }

    public bool IsActiveOn(DateOnly date)
    {
        return ActivePeriod.Contains(date);
    }
}
