using MacroViva.Application.Abstractions.Services;
using MacroViva.Infrastructure.Persistence;
using MacroViva.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace MacroViva.Api.Tests;

public sealed class AnonymousBetaUserResolverTests
{
    private const string HashingKey = "test-only-beta-identity-hashing-key-123456";

    [Fact]
    public void DeriveUserId_ReturnsSameIdForTheSameInstallation()
    {
        var installationId = Guid.Parse("11111111-1111-4111-8111-111111111111");

        var first = AnonymousBetaUserResolver.DeriveUserId(installationId, HashingKey);
        var second = AnonymousBetaUserResolver.DeriveUserId(installationId, HashingKey);

        Assert.Equal(first, second);
    }

    [Fact]
    public void DeriveUserId_ReturnsDifferentIdsForDifferentInstallations()
    {
        var first = AnonymousBetaUserResolver.DeriveUserId(
            Guid.Parse("11111111-1111-4111-8111-111111111111"),
            HashingKey);
        var second = AnonymousBetaUserResolver.DeriveUserId(
            Guid.Parse("22222222-2222-4222-8222-222222222222"),
            HashingKey);

        Assert.NotEqual(first, second);
    }

    [Fact]
    public async Task ResolveOrCreateAsync_CreatesAndThenReusesTheTechnicalUser()
    {
        await using var dbContext = CreateDbContext();
        var resolver = CreateResolver(dbContext);
        var installationId = Guid.Parse("33333333-3333-4333-8333-333333333333");

        var first = await resolver.ResolveOrCreateAsync(installationId, CancellationToken.None);
        var second = await resolver.ResolveOrCreateAsync(installationId, CancellationToken.None);

        Assert.Equal(first, second);
        Assert.Single(dbContext.Users);
        Assert.Equal($"beta-{first:N}@anonymous.macroviva.invalid", dbContext.Users.Single().Email);
    }

    [Fact]
    public async Task ResolveOrCreateAsync_ConcurrentCallsCreateOnlyOneTechnicalUser()
    {
        var databaseName = Guid.NewGuid().ToString("N");
        var databaseRoot = new InMemoryDatabaseRoot();
        var installationId = Guid.Parse("44444444-4444-4444-8444-444444444444");

        await using var firstContext = CreateDbContext(databaseName, databaseRoot);
        await using var secondContext = CreateDbContext(databaseName, databaseRoot);
        var firstResolver = CreateResolver(firstContext);
        var secondResolver = CreateResolver(secondContext);

        var userIds = await Task.WhenAll(
            firstResolver.ResolveOrCreateAsync(installationId, CancellationToken.None),
            secondResolver.ResolveOrCreateAsync(installationId, CancellationToken.None));

        await using var verificationContext = CreateDbContext(databaseName, databaseRoot);
        Assert.Equal(userIds[0], userIds[1]);
        Assert.Single(verificationContext.Users);
    }

    private static MacroVivaDbContext CreateDbContext()
    {
        return CreateDbContext(Guid.NewGuid().ToString("N"), new InMemoryDatabaseRoot());
    }

    private static MacroVivaDbContext CreateDbContext(string databaseName, InMemoryDatabaseRoot databaseRoot)
    {
        var options = new DbContextOptionsBuilder<MacroVivaDbContext>()
            .UseInMemoryDatabase(databaseName, databaseRoot)
            .Options;

        return new MacroVivaDbContext(options);
    }

    private static AnonymousBetaUserResolver CreateResolver(MacroVivaDbContext dbContext)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["BetaTesterIdentity:HashingKey"] = HashingKey
            })
            .Build();

        return new AnonymousBetaUserResolver(dbContext, new FixedClock(), configuration);
    }

    private sealed class FixedClock : IClock
    {
        public DateTimeOffset UtcNow => new(2026, 7, 14, 12, 0, 0, TimeSpan.Zero);

        public DateOnly Today => DateOnly.FromDateTime(UtcNow.UtcDateTime);
    }
}
