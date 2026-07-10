using MacroViva.Application.Abstractions.Repositories;
using MacroViva.Domain.Nutrition;
using MacroViva.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MacroViva.Infrastructure.Repositories;

public sealed class FoodRepository(MacroVivaDbContext dbContext) : IFoodRepository
{
    public async Task<IReadOnlyList<Food>> SearchAsync(string? search, CancellationToken cancellationToken)
    {
        var query = dbContext.Foods
            .AsNoTracking()
            .Include(food => food.Portions)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            query = query.Where(food => EF.Functions.Like(food.Name.Value, pattern));
        }

        return await query
            .OrderBy(food => food.Name.Value)
            .Take(50)
            .ToListAsync(cancellationToken);
    }

    public Task<Food?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Foods
            .Include(food => food.Portions)
            .FirstOrDefaultAsync(food => food.Id == id, cancellationToken);
    }
}
