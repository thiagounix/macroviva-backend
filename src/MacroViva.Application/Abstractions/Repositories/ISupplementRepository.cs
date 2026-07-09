using MacroViva.Domain.Supplements;

namespace MacroViva.Application.Abstractions.Repositories;

public interface ISupplementRepository
{
    Task<IReadOnlyList<Supplement>> GetAllAsync(CancellationToken cancellationToken);

    Task<Supplement?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
