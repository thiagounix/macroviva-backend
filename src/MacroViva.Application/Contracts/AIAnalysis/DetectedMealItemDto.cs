using MacroViva.Domain.Enums;

namespace MacroViva.Application.Contracts.AIAnalysis;

public sealed record DetectedMealItemDto(
    Guid AnalysisItemId,
    string SuggestedFoodName,
    decimal Grams,
    ConfidenceLevel ConfidenceLevel,
    decimal ConfidenceScore,
    Guid? SuggestedFoodId);
