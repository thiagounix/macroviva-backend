using MacroViva.Domain.Enums;

namespace MacroViva.Application.Abstractions.Services;

public sealed record MealVisionDetectedItem(
    string SuggestedFoodName,
    decimal Grams,
    ConfidenceLevel ConfidenceLevel,
    decimal ConfidenceScore,
    Guid? SuggestedFoodId);
