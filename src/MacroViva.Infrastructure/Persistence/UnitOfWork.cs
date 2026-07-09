using MacroViva.Application.Abstractions.Repositories;

namespace MacroViva.Infrastructure.Persistence;

public sealed class UnitOfWork(MacroVivaDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
