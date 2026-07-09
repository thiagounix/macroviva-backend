using MacroViva.Domain.Common;
using MacroViva.Domain.Enums;
using MacroViva.Domain.ValueObjects;

namespace MacroViva.Domain.Supplements;

public sealed class Supplement : AggregateRoot
{
    private Supplement(
        Guid id,
        LocalizedName name,
        SupplementType type,
        Macronutrients macronutrientsPerServing)
        : base(id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Type = type;
        MacronutrientsPerServing = macronutrientsPerServing ?? throw new ArgumentNullException(nameof(macronutrientsPerServing));
    }

    public LocalizedName Name { get; private set; }

    public SupplementType Type { get; }

    public Macronutrients MacronutrientsPerServing { get; private set; }

    public bool ImpactsMacronutrients => MacronutrientsPerServing != Macronutrients.Zero;

    public static Supplement Create(
        Guid id,
        LocalizedName name,
        SupplementType type,
        Macronutrients macronutrientsPerServing)
    {
        return new Supplement(id, name, type, macronutrientsPerServing);
    }

    public static Supplement CreateWheyProtein(Guid id, LocalizedName name, Macronutrients macronutrientsPerServing)
    {
        if (name is null)
        {
            throw new DomainException("Whey protein name must be provided.");
        }

        if (macronutrientsPerServing is null)
        {
            throw new DomainException("Whey protein macros must be provided.");
        }

        if (macronutrientsPerServing.ProteinGrams <= 0)
        {
            throw new DomainException("Whey protein must have protein macros.");
        }

        return new Supplement(id, name, SupplementType.WheyProtein, macronutrientsPerServing);
    }

    public static Supplement CreateCreatine(Guid id, LocalizedName name)
    {
        return new Supplement(id, name, SupplementType.Creatine, Macronutrients.Zero);
    }

    public Macronutrients CalculateImpact(decimal servings)
    {
        Guard.AgainstZeroOrNegative(servings, nameof(servings));

        return MacronutrientsPerServing.ScaleBy(servings);
    }
}
