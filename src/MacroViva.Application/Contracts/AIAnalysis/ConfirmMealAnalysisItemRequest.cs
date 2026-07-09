namespace MacroViva.Application.Contracts.AIAnalysis;

public sealed record ConfirmMealAnalysisItemRequest(Guid AnalysisItemId, Guid SelectedFoodId, decimal Grams);
