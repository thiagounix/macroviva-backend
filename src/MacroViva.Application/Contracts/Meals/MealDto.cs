using MacroViva.Application.Contracts.Common;
using MacroViva.Domain.Enums;

namespace MacroViva.Application.Contracts.Meals;

public sealed record MealDto(
    Guid Id,
    Guid UserId,
    MealType MealType,
    DateTimeOffset OccurredAt,
    MacronutrientsDto TotalMacronutrients,
    IReadOnlyList<MealItemDto> Items);
