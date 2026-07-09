namespace MacroViva.Domain.ValueObjects;

public sealed record NutritionPer100g
{
    private NutritionPer100g()
    {
        Macronutrients = null!;
    }

    public NutritionPer100g(Macronutrients macronutrients)
    {
        Macronutrients = macronutrients ?? throw new ArgumentNullException(nameof(macronutrients));
    }

    public Macronutrients Macronutrients { get; }

    public Macronutrients CalculateFor(Portion portion)
    {
        ArgumentNullException.ThrowIfNull(portion);

        return Macronutrients.ScaleBy(portion.Grams / 100m);
    }
}
