using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using MacroViva.Application.Abstractions.Services;
using MacroViva.Domain.Enums;
using MacroViva.Domain.Users;
using MacroViva.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MacroViva.Infrastructure.Services;

public sealed class AnonymousBetaUserResolver(
    MacroVivaDbContext dbContext,
    IClock clock,
    IConfiguration configuration) : IAnonymousBetaUserResolver
{
    private static readonly ConcurrentDictionary<Guid, SemaphoreSlim> UserCreationLocks = new();

    public async Task<Guid> ResolveOrCreateAsync(Guid installationId, CancellationToken cancellationToken)
    {
        var userId = DeriveUserId(installationId, GetHashingKey());

        if (await dbContext.Users.AnyAsync(user => user.Id == userId, cancellationToken))
        {
            return userId;
        }

        var creationLock = UserCreationLocks.GetOrAdd(userId, _ => new SemaphoreSlim(1, 1));
        await creationLock.WaitAsync(cancellationToken);

        try
        {
            if (await dbContext.Users.AnyAsync(user => user.Id == userId, cancellationToken))
            {
                return userId;
            }

            var user = User.Create(
                userId,
                $"beta-{userId:N}@anonymous.macroviva.invalid",
                LocaleCode.PtBr,
                clock.UtcNow);

            dbContext.Users.Add(user);

            try
            {
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                dbContext.ChangeTracker.Clear();

                if (!await dbContext.Users.AnyAsync(existingUser => existingUser.Id == userId, cancellationToken))
                {
                    throw;
                }
            }

            return userId;
        }
        finally
        {
            creationLock.Release();
        }
    }

    public static Guid DeriveUserId(Guid installationId, string hashingKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hashingKey);

        var hash = HMACSHA256.HashData(
            Encoding.UTF8.GetBytes(hashingKey),
            installationId.ToByteArray());

        return new Guid(hash.AsSpan(0, 16));
    }

    private string GetHashingKey()
    {
        return configuration["BetaTesterIdentity:HashingKey"]
            ?? throw new InvalidOperationException("BetaTesterIdentity:HashingKey is required in Staging.");
    }
}
