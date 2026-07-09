using MacroViva.Domain.Supplements;

namespace MacroViva.Application.Abstractions.Repositories;

public interface IUserSupplementRepository
{
    Task AddAsync(UserSupplement userSupplement, CancellationToken cancellationToken);

    Task<UserSupplement?> GetByUserAndSupplementAsync(Guid userId, Guid supplementId, CancellationToken cancellationToken);

    void Update(UserSupplement userSupplement);
}
