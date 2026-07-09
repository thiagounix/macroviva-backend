using MacroViva.Domain.Common;

namespace MacroViva.Domain.ValueObjects;

public sealed record Portion
{
    private Portion()
    {
        Unit = string.Empty;
    }

    private Portion(decimal quantity, string unit, decimal grams)
    {
        Quantity = Guard.AgainstZeroOrNegative(quantity, nameof(quantity));
        Unit = Guard.AgainstNullOrWhiteSpace(unit, nameof(unit));
        Grams = Guard.AgainstZeroOrNegative(grams, nameof(grams));
    }

    public decimal Quantity { get; }

    public string Unit { get; }

    public decimal Grams { get; }

    public static Portion FromGrams(decimal grams)
    {
        return new Portion(grams, "g", grams);
    }

    public static Portion FromServing(decimal servings, decimal gramsPerServing)
    {
        return new Portion(servings, "serving", servings * Guard.AgainstZeroOrNegative(gramsPerServing, nameof(gramsPerServing)));
    }
}
