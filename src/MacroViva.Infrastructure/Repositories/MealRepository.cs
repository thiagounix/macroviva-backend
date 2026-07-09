using MacroViva.Application.Abstractions.Repositories;
using MacroViva.Domain.Meals;
using MacroViva.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MacroViva.Infrastructure.Repositories;

public sealed class MealRepository(MacroVivaDbContext dbContext) : IMealRepository
{
    public async Task AddAsync(Meal meal, CancellationToken cancellationToken)
    {
        await dbContext.Meals.AddAsync(meal, cancellationToken);
    }

    public async Task<IReadOnlyList<Meal>> GetByUserAndDateAsync(Guid userId, DateOnly date, CancellationToken cancellationToken)
    {
        var startsAt = new DateTimeOffset(date.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        var endsAt = startsAt.AddDays(1);

        return await dbContext.Meals
            .AsNoTracking()
            .Include(meal => meal.Items)
            .Include(meal => meal.Photos)
            .Where(meal => meal.UserId == userId && meal.OccurredAt >= startsAt && meal.OccurredAt < endsAt)
            .OrderBy(meal => meal.OccurredAt)
            .ToListAsync(cancellationToken);
    }
}
