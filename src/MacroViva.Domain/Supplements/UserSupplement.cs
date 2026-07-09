using MacroViva.Domain.Common;
using MacroViva.Domain.ValueObjects;

namespace MacroViva.Domain.Supplements;

public sealed class UserSupplement : AggregateRoot
{
    private UserSupplement()
    {
        MacronutrientImpact = null!;
    }

    private UserSupplement(
        Guid id,
        Guid userId,
        Guid supplementId,
        DateOnly checkInDate,
        decimal servings,
        Macronutrients macronutrientImpact)
        : base(id)
    {
        UserId = Guard.AgainstEmpty(userId, nameof(userId));
        SupplementId = Guard.AgainstEmpty(supplementId, nameof(supplementId));
        CheckInDate = checkInDate;
        Servings = Guard.AgainstZeroOrNegative(servings, nameof(servings));
        MacronutrientImpact = macronutrientImpact ?? throw new ArgumentNullException(nameof(macronutrientImpact));
    }

    public Guid UserId { get; }

    public Guid SupplementId { get; }

    public DateOnly CheckInDate { get; }

    public decimal Servings { get; }

    public Macronutrients MacronutrientImpact { get; }

    public static UserSupplement CheckIn(
        Guid id,
        Guid userId,
        Supplement supplement,
        DateOnly checkInDate,
        decimal servings)
    {
        ArgumentNullException.ThrowIfNull(supplement);

        return new UserSupplement(
            id,
            userId,
            supplement.Id,
            checkInDate,
            servings,
            supplement.CalculateImpact(servings));
    }
}
