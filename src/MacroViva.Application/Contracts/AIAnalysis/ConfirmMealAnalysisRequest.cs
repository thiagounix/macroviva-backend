using MacroViva.Domain.Enums;

namespace MacroViva.Application.Contracts.AIAnalysis;

public sealed record ConfirmMealAnalysisRequest(
    Guid AnalysisId,
    MealType MealType,
    DateTimeOffset OccurredAt,
    IReadOnlyList<ConfirmMealAnalysisItemRequest> Items);
