using MacroViva.Domain.ValueObjects;

namespace MacroViva.Domain.Tests;

public sealed class MacronutrientCalculationTests
{
    [Fact]
    public void ShouldCalculateMacrosByGramsFromNutritionPer100g()
    {
        var nutrition = new NutritionPer100g(new Macronutrients(
            calories: 130m,
            proteinGrams: 2.7m,
            carbohydrateGrams: 28m,
            fatGrams: 0.3m));

        var macros = nutrition.CalculateFor(Portion.FromGrams(150m));

        Assert.Equal(195m, macros.Calories);
        Assert.Equal(4.05m, macros.ProteinGrams);
        Assert.Equal(42m, macros.CarbohydrateGrams);
        Assert.Equal(0.45m, macros.FatGrams);
    }
}
