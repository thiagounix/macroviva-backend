using MacroViva.Application.Contracts.Common;
using MacroViva.Domain.Enums;

namespace MacroViva.Application.Contracts.Foods;

public sealed record FoodDto(
    Guid Id,
    string Name,
    LocaleCode Locale,
    FoodCategory Category,
    MacronutrientsDto NutritionPer100g,
    bool IsSupplement);
