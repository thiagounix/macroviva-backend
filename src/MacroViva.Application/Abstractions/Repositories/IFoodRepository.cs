using MacroViva.Domain.Nutrition;

namespace MacroViva.Application.Abstractions.Repositories;

public interface IFoodRepository
{
    Task<IReadOnlyList<Food>> SearchAsync(string? search, CancellationToken cancellationToken);

    Task<Food?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
