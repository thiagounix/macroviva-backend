using MacroViva.Domain.Meals;

namespace MacroViva.Application.Abstractions.Repositories;

public interface IMealRepository
{
    Task AddAsync(Meal meal, CancellationToken cancellationToken);

    Task<IReadOnlyList<Meal>> GetByUserAndDateAsync(Guid userId, DateOnly date, CancellationToken cancellationToken);
}
