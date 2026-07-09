namespace MacroViva.Application.Abstractions.Services;

public sealed record MealVisionAnalysisResult(IReadOnlyList<MealVisionDetectedItem> Items);
