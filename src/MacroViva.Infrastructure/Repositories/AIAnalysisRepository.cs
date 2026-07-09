using MacroViva.Application.Abstractions.Repositories;
using MacroViva.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MealImageAnalysis = MacroViva.Domain.AIAnalysis.AIAnalysis;

namespace MacroViva.Infrastructure.Repositories;

public sealed class AIAnalysisRepository(MacroVivaDbContext dbContext) : IAIAnalysisRepository
{
    public async Task AddAsync(MealImageAnalysis analysis, CancellationToken cancellationToken)
    {
        await dbContext.AIAnalyses.AddAsync(analysis, cancellationToken);
    }

    public Task<MealImageAnalysis?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.AIAnalyses
            .Include(analysis => analysis.Items)
            .FirstOrDefaultAsync(analysis => analysis.Id == id, cancellationToken);
    }

    public void Update(MealImageAnalysis analysis)
    {
        dbContext.AIAnalyses.Update(analysis);
    }
}
