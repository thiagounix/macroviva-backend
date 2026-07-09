using MacroViva.Domain.Enums;

namespace MacroViva.Application.Contracts.Meals;

public sealed record CreateMealRequest(
    MealType MealType,
    DateTimeOffset OccurredAt,
    IReadOnlyList<CreateMealItemRequest> Items);
