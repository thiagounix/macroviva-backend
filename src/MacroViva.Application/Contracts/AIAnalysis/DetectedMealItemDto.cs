using MacroViva.Domain.Enums;

namespace MacroViva.Application.Contracts.AIAnalysis;

public sealed record DetectedMealItemDto(
    string SuggestedFoodName,
    decimal Grams,
    ConfidenceLevel ConfidenceLevel,
    decimal ConfidenceScore,
    Guid? SuggestedFoodId);
