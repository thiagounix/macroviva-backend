using MacroViva.Application.Contracts.Common;

namespace MacroViva.Application.Contracts.Foods;

public sealed record FoodPortionDto(
    Guid Id,
    string Name,
    string Label,
    decimal Grams,
    MacronutrientsDto Macronutrients);
