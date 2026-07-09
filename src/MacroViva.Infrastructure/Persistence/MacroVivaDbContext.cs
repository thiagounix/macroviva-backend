using MacroViva.Domain.AIAnalysis;
using MacroViva.Domain.ConsentAndPrivacy;
using MacroViva.Domain.Meals;
using MacroViva.Domain.Nutrition;
using MacroViva.Domain.Subscriptions;
using MacroViva.Domain.Supplements;
using MacroViva.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace MacroViva.Infrastructure.Persistence;

public sealed class MacroVivaDbContext(DbContextOptions<MacroVivaDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    public DbSet<UserGoal> UserGoals => Set<UserGoal>();

    public DbSet<UserConsent> UserConsents => Set<UserConsent>();

    public DbSet<Food> Foods => Set<Food>();

    public DbSet<FoodPortion> FoodPortions => Set<FoodPortion>();

    public DbSet<Meal> Meals => Set<Meal>();

    public DbSet<MealItem> MealItems => Set<MealItem>();

    public DbSet<MealPhoto> MealPhotos => Set<MealPhoto>();

    public DbSet<AIAnalysis> AIAnalyses => Set<AIAnalysis>();

    public DbSet<AIAnalysisItem> AIAnalysisItems => Set<AIAnalysisItem>();

    public DbSet<Supplement> Supplements => Set<Supplement>();

    public DbSet<UserSupplement> UserSupplements => Set<UserSupplement>();

    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();

    public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MacroVivaDbContext).Assembly);
    }
}
