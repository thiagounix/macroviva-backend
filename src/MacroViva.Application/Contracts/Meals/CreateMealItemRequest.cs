namespace MacroViva.Application.Contracts.Meals;

public sealed record CreateMealItemRequest(Guid FoodId, decimal Grams);
