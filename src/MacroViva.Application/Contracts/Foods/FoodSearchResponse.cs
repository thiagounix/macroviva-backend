namespace MacroViva.Application.Contracts.Foods;

public sealed record FoodSearchResponse(IReadOnlyList<FoodDto> Foods);
