using MealImageAnalysis = MacroViva.Domain.AIAnalysis.AIAnalysis;

namespace MacroViva.Application.Abstractions.Repositories;

public interface IAIAnalysisRepository
{
    Task AddAsync(MealImageAnalysis analysis, CancellationToken cancellationToken);

    Task<MealImageAnalysis?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    void Update(MealImageAnalysis analysis);
}
