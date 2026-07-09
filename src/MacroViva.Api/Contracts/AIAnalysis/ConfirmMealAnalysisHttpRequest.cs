using MacroViva.Application.Contracts.AIAnalysis;
using MacroViva.Domain.Enums;

namespace MacroViva.Api.Contracts.AIAnalysis;

public sealed record ConfirmMealAnalysisHttpRequest(
    MealType MealType,
    DateTimeOffset OccurredAt,
    IReadOnlyList<ConfirmMealAnalysisItemRequest> Items);
