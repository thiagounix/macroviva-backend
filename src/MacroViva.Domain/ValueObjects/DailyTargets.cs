using MacroViva.Domain.Common;

namespace MacroViva.Domain.ValueObjects;

public sealed record DailyTargets
{
    public DailyTargets(Macronutrients targetMacronutrients)
    {
        TargetMacronutrients = targetMacronutrients ?? throw new ArgumentNullException(nameof(targetMacronutrients));

        if (TargetMacronutrients.Calories <= 0)
        {
            throw new DomainException("Daily calorie target must be greater than zero.");
        }
    }

    public Macronutrients TargetMacronutrients { get; }
}
