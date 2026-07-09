using MacroViva.Application.Abstractions.Repositories;
using MacroViva.Domain.Supplements;
using MacroViva.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MacroViva.Infrastructure.Repositories;

public sealed class SupplementRepository(MacroVivaDbContext dbContext) : ISupplementRepository
{
    public async Task<IReadOnlyList<Supplement>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Supplements
            .AsNoTracking()
            .OrderBy(supplement => supplement.Name.Value)
            .ToListAsync(cancellationToken);
    }

    public Task<Supplement?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Supplements.FirstOrDefaultAsync(supplement => supplement.Id == id, cancellationToken);
    }
}
