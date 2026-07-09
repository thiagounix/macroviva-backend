using MacroViva.Domain.Enums;
using MacroViva.Domain.Meals;
using MacroViva.Domain.Nutrition;
using MacroViva.Domain.ValueObjects;

namespace MacroViva.Domain.Tests;

public sealed class MealTests
{
    private static readonly DateTimeOffset OccurredAt = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void ShouldTotalMealWithMultipleItems()
    {
        var userId = Guid.NewGuid();
        var meal = Meal.Create(Guid.NewGuid(), userId, MealType.Lunch, OccurredAt);
        var chicken = CreateFood("Chicken breast", FoodCategory.Protein, new Macronutrients(165m, 31m, 0m, 3.6m));
        var rice = CreateFood("Rice", FoodCategory.Grain, new Macronutrients(130m, 2.7m, 28m, 0.3m));

        meal.AddFood(Guid.NewGuid(), chicken, Portion.FromGrams(200m));
        meal.AddFood(Guid.NewGuid(), rice, Portion.FromGrams(150m));

        Assert.Equal(525m, meal.TotalMacronutrients.Calories);
        Assert.Equal(66.05m, meal.TotalMacronutrients.ProteinGrams);
        Assert.Equal(42m, meal.TotalMacronutrients.CarbohydrateGrams);
        Assert.Equal(7.65m, meal.TotalMacronutrients.FatGrams);
    }

    [Fact]
    public void ShouldKeepNutritionSnapshotInMealItem()
    {
        var meal = Meal.Create(Guid.NewGuid(), Guid.NewGuid(), MealType.Dinner, OccurredAt);
        var food = CreateFood("Chicken breast", FoodCategory.Protein, new Macronutrients(165m, 31m, 0m, 3.6m));

        var item = meal.AddFood(Guid.NewGuid(), food, Portion.FromGrams(100m));

        food.Rename(new LocalizedName("Updated chicken", LocaleCode.EnUs));
        food.UpdateNutrition(new NutritionPer100g(new Macronutrients(250m, 10m, 20m, 10m)));

        Assert.Equal("Chicken breast", item.FoodNameSnapshot.Value);
        Assert.Equal(165m, item.MacronutrientsSnapshot.Calories);
        Assert.Equal(31m, item.MacronutrientsSnapshot.ProteinGrams);
        Assert.Equal(0m, item.MacronutrientsSnapshot.CarbohydrateGrams);
        Assert.Equal(3.6m, item.MacronutrientsSnapshot.FatGrams);
    }

    private static Food CreateFood(string name, FoodCategory category, Macronutrients macrosPer100g)
    {
        return Food.Create(
            Guid.NewGuid(),
            new LocalizedName(name, LocaleCode.EnUs),
            category,
            new NutritionPer100g(macrosPer100g));
    }
}
