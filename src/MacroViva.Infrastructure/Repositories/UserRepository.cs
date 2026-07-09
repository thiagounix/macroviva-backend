using MacroViva.Application.Abstractions.Repositories;
using MacroViva.Domain.Users;
using MacroViva.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MacroViva.Infrastructure.Repositories;

public sealed class UserRepository(MacroVivaDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Users
            .Include(user => user.Profile)
            .Include(user => user.Goal)
            .Include(user => user.Consents)
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }
}
