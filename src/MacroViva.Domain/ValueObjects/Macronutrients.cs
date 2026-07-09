using MacroViva.Domain.Common;

namespace MacroViva.Domain.ValueObjects;

public sealed record Macronutrients
{
    public static Macronutrients Zero { get; } = new(0, 0, 0, 0);

    public Macronutrients(decimal calories, decimal proteinGrams, decimal carbohydrateGrams, decimal fatGrams)
    {
        Calories = Guard.AgainstNegative(calories, nameof(calories));
        ProteinGrams = Guard.AgainstNegative(proteinGrams, nameof(proteinGrams));
        CarbohydrateGrams = Guard.AgainstNegative(carbohydrateGrams, nameof(carbohydrateGrams));
        FatGrams = Guard.AgainstNegative(fatGrams, nameof(fatGrams));
    }

    public decimal Calories { get; }

    public decimal ProteinGrams { get; }

    public decimal CarbohydrateGrams { get; }

    public decimal FatGrams { get; }

    public Macronutrients Add(Macronutrients other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return new Macronutrients(
            Calories + other.Calories,
            ProteinGrams + other.ProteinGrams,
            CarbohydrateGrams + other.CarbohydrateGrams,
            FatGrams + other.FatGrams);
    }

    public Macronutrients ScaleBy(decimal factor)
    {
        Guard.AgainstNegative(factor, nameof(factor));

        return new Macronutrients(
            Calories * factor,
            ProteinGrams * factor,
            CarbohydrateGrams * factor,
            FatGrams * factor);
    }

    public static Macronutrients operator +(Macronutrients left, Macronutrients right)
    {
        return left.Add(right);
    }
}
