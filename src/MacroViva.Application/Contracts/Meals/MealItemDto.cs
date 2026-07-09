using MacroViva.Application.Contracts.Common;

namespace MacroViva.Application.Contracts.Meals;

public sealed record MealItemDto(
    Guid Id,
    Guid FoodId,
    string FoodName,
    decimal Grams,
    MacronutrientsDto Macronutrients);
