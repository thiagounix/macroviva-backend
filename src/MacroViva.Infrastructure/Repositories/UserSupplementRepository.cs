using MacroViva.Application.Abstractions.Repositories;
using MacroViva.Domain.Supplements;
using MacroViva.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MacroViva.Infrastructure.Repositories;

public sealed class UserSupplementRepository(MacroVivaDbContext dbContext) : IUserSupplementRepository
{
    public async Task AddAsync(UserSupplement userSupplement, CancellationToken cancellationToken)
    {
        await dbContext.UserSupplements.AddAsync(userSupplement, cancellationToken);
    }

    public Task<UserSupplement?> GetByUserAndSupplementAsync(Guid userId, Guid supplementId, CancellationToken cancellationToken)
    {
        return dbContext.UserSupplements
            .FirstOrDefaultAsync(
                userSupplement => userSupplement.UserId == userId && userSupplement.SupplementId == supplementId,
                cancellationToken);
    }

    public void Update(UserSupplement userSupplement)
    {
        dbContext.UserSupplements.Update(userSupplement);
    }
}
