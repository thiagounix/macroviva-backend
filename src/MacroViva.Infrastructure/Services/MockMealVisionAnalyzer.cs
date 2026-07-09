using MacroViva.Application.Abstractions.Services;
using MacroViva.Domain.Enums;

namespace MacroViva.Infrastructure.Services;

public sealed class MockMealVisionAnalyzer : IMealVisionAnalyzer
{
    public Task<MealVisionAnalysisResult> AnalyzeAsync(MealVisionAnalysisRequest request, CancellationToken cancellationToken)
    {
        IReadOnlyList<MealVisionDetectedItem> items =
        [
            new("Arroz branco cozido", 120m, ConfidenceLevel.High, 0.91m, null),
            new("Feijao carioca cozido", 90m, ConfidenceLevel.Medium, 0.78m, null),
            new("Peito de frango grelhado", 130m, ConfidenceLevel.High, 0.88m, null)
        ];

        return Task.FromResult(new MealVisionAnalysisResult(items));
    }
}
