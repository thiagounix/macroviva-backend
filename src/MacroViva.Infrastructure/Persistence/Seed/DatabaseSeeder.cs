using MacroViva.Domain.Enums;
using MacroViva.Domain.Nutrition;
using MacroViva.Domain.Subscriptions;
using MacroViva.Domain.Supplements;
using MacroViva.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace MacroViva.Infrastructure.Persistence.Seed;

public sealed class DatabaseSeeder(MacroVivaDbContext dbContext)
{
    public async Task SeedDevelopmentDataAsync(CancellationToken cancellationToken)
    {
        await AddMissingFoodsAsync(cancellationToken);
        await AddMissingSupplementsAsync(cancellationToken);
        await AddMissingSubscriptionPlansAsync(cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task AddMissingFoodsAsync(CancellationToken cancellationToken)
    {
        var foods = CreateFoods();
        var foodIds = foods.Select(food => food.Id).ToList();
        var existingIds = await dbContext.Foods
            .Where(food => foodIds.Contains(food.Id))
            .Select(food => food.Id)
            .ToListAsync(cancellationToken);

        dbContext.Foods.AddRange(foods.Where(food => !existingIds.Contains(food.Id)));
    }

    private async Task AddMissingSupplementsAsync(CancellationToken cancellationToken)
    {
        var supplements = CreateSupplements();
        var supplementIds = supplements.Select(supplement => supplement.Id).ToList();
        var existingIds = await dbContext.Supplements
            .Where(supplement => supplementIds.Contains(supplement.Id))
            .Select(supplement => supplement.Id)
            .ToListAsync(cancellationToken);

        dbContext.Supplements.AddRange(supplements.Where(supplement => !existingIds.Contains(supplement.Id)));
    }

    private async Task AddMissingSubscriptionPlansAsync(CancellationToken cancellationToken)
    {
        var plans = CreateSubscriptionPlans();
        var planIds = plans.Select(plan => plan.Id).ToList();
        var existingIds = await dbContext.SubscriptionPlans
            .Where(plan => planIds.Contains(plan.Id))
            .Select(plan => plan.Id)
            .ToListAsync(cancellationToken);

        dbContext.SubscriptionPlans.AddRange(plans.Where(plan => !existingIds.Contains(plan.Id)));
    }

    private static IReadOnlyList<Food> CreateFoods()
    {
        return
        [
            Food.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000001"),
                Name("Banana prata"),
                FoodCategory.Fruit,
                Nutrition(89m, 1.3m, 22.8m, 0.1m)),
            Food.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000002"),
                Name("Banana nanica"),
                FoodCategory.Fruit,
                Nutrition(92m, 1.4m, 23.8m, 0.1m)),
            Food.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000003"),
                Name("Maca"),
                FoodCategory.Fruit,
                Nutrition(52m, 0.3m, 14m, 0.2m)),
            Food.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000004"),
                Name("Arroz branco cozido"),
                FoodCategory.Grain,
                Nutrition(130m, 2.7m, 28m, 0.3m)),
            Food.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000005"),
                Name("Feijao carioca cozido"),
                FoodCategory.Legume,
                Nutrition(76m, 4.8m, 13.6m, 0.5m)),
            Food.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000006"),
                Name("Peito de frango grelhado"),
                FoodCategory.Protein,
                Nutrition(165m, 31m, 0m, 3.6m)),
            Food.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000007"),
                Name("Ovo cozido"),
                FoodCategory.Protein,
                Nutrition(155m, 13m, 1.1m, 11m)),
            Food.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000008"),
                Name("Whey protein generico"),
                FoodCategory.Supplement,
                Nutrition(400m, 80m, 8m, 6m),
                isSupplement: true)
        ];
    }

    private static IReadOnlyList<Supplement> CreateSupplements()
    {
        return
        [
            Supplement.CreateCreatine(
                Guid.Parse("20000000-0000-0000-0000-000000000001"),
                Name("Creatina monohidratada generica")),
            Supplement.CreateWheyProtein(
                Guid.Parse("20000000-0000-0000-0000-000000000002"),
                Name("Whey protein generico"),
                new Macronutrients(120m, 24m, 3m, 2m))
        ];
    }

    private static IReadOnlyList<SubscriptionPlan> CreateSubscriptionPlans()
    {
        var startsOn = new DateOnly(2026, 1, 1);

        return
        [
            SubscriptionPlan.Create(
                Guid.Parse("30000000-0000-0000-0000-000000000001"),
                Name("Free"),
                SubscriptionTier.Free,
                new Money(0m, "BRL"),
                new DateRange(startsOn)),
            SubscriptionPlan.Create(
                Guid.Parse("30000000-0000-0000-0000-000000000002"),
                Name("Plus"),
                SubscriptionTier.Plus,
                new Money(19.90m, "BRL"),
                new DateRange(startsOn)),
            SubscriptionPlan.Create(
                Guid.Parse("30000000-0000-0000-0000-000000000003"),
                Name("Pro"),
                SubscriptionTier.Premium,
                new Money(39.90m, "BRL"),
                new DateRange(startsOn))
        ];
    }

    private static LocalizedName Name(string value)
    {
        return new LocalizedName(value, LocaleCode.PtBr);
    }

    private static NutritionPer100g Nutrition(
        decimal calories,
        decimal proteinGrams,
        decimal carbohydrateGrams,
        decimal fatGrams)
    {
        return new NutritionPer100g(new Macronutrients(calories, proteinGrams, carbohydrateGrams, fatGrams));
    }
}
