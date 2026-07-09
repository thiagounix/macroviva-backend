using MacroViva.Domain.Common;
using MacroViva.Domain.Nutrition;
using MacroViva.Domain.ValueObjects;

namespace MacroViva.Domain.Meals;

public sealed class MealItem : Entity
{
    private MealItem()
    {
        FoodNameSnapshot = null!;
        Portion = null!;
        MacronutrientsSnapshot = null!;
    }

    private MealItem(
        Guid id,
        Guid mealId,
        Guid foodId,
        LocalizedName foodNameSnapshot,
        Portion portion,
        Macronutrients macronutrientsSnapshot)
        : base(id)
    {
        MealId = Guard.AgainstEmpty(mealId, nameof(mealId));
        FoodId = Guard.AgainstEmpty(foodId, nameof(foodId));
        FoodNameSnapshot = foodNameSnapshot ?? throw new ArgumentNullException(nameof(foodNameSnapshot));
        Portion = portion ?? throw new ArgumentNullException(nameof(portion));
        MacronutrientsSnapshot = macronutrientsSnapshot ?? throw new ArgumentNullException(nameof(macronutrientsSnapshot));
    }

    public Guid MealId { get; }

    public Guid FoodId { get; }

    public LocalizedName FoodNameSnapshot { get; }

    public Portion Portion { get; }

    public Macronutrients MacronutrientsSnapshot { get; }

    internal static MealItem FromFood(Guid id, Guid mealId, Food food, Portion portion)
    {
        ArgumentNullException.ThrowIfNull(food);
        ArgumentNullException.ThrowIfNull(portion);

        return new MealItem(
            id,
            mealId,
            food.Id,
            food.Name,
            portion,
            food.CalculateFor(portion));
    }
}
