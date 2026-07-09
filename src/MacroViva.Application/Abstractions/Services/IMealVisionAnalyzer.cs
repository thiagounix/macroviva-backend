namespace MacroViva.Application.Abstractions.Services;

public interface IMealVisionAnalyzer
{
    Task<MealVisionAnalysisResult> AnalyzeAsync(MealVisionAnalysisRequest request, CancellationToken cancellationToken);
}
