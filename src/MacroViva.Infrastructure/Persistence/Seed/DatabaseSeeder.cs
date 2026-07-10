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
        await AddMissingFoodPortionsAsync(cancellationToken);
        await AddMissingSupplementsAsync(cancellationToken);
        await AddMissingSubscriptionPlansAsync(cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task AddMissingFoodsAsync(CancellationToken cancellationToken)
    {
        var foods = CreateFoods();
        var foodIds = foods.Select(food => food.Id).ToList();
        var existingFoods = await dbContext.Foods
            .Where(food => foodIds.Contains(food.Id))
            .Select(food => new
            {
                food.Id,
                Name = food.Name.Value,
                food.Category,
                food.IsSupplement
            })
            .ToListAsync(cancellationToken);

        var seedNames = foods.Select(food => food.Name.Value).ToList();
        var existingFoodsByNaturalKey = await dbContext.Foods
            .Where(food => seedNames.Contains(food.Name.Value))
            .Select(food => new
            {
                food.Id,
                Name = food.Name.Value,
                food.Category,
                food.IsSupplement
            })
            .ToListAsync(cancellationToken);

        var existingKeys = existingFoods
            .Concat(existingFoodsByNaturalKey)
            .SelectMany(food => new[]
            {
                FoodKey(food.Id, food.Name, food.Category, food.IsSupplement),
                FoodNaturalKey(food.Name, food.Category, food.IsSupplement)
            })
            .ToHashSet();
        var existingIds = existingFoods
            .Select(food => food.Id)
            .ToHashSet();

        dbContext.Foods.AddRange(foods.Where(food =>
            !existingIds.Contains(food.Id) &&
            !existingKeys.Contains(FoodKey(food.Id, food.Name.Value, food.Category, food.IsSupplement)) &&
            !existingKeys.Contains(FoodNaturalKey(food.Name.Value, food.Category, food.IsSupplement))));

        var existingFoodEntities = await dbContext.Foods
            .Where(food => foodIds.Contains(food.Id))
            .ToListAsync(cancellationToken);

        foreach (var existingFood in existingFoodEntities)
        {
            var seedFood = foods.First(food => food.Id == existingFood.Id);

            if (existingFood.Name.Value != seedFood.Name.Value)
            {
                existingFood.Rename(seedFood.Name);
            }

            if (existingFood.NutritionPer100g != seedFood.NutritionPer100g)
            {
                existingFood.UpdateNutrition(seedFood.NutritionPer100g);
            }
        }
    }

    private async Task AddMissingFoodPortionsAsync(CancellationToken cancellationToken)
    {
        var portions = CreateFoodPortions();
        var portionIds = portions.Select(portion => portion.Id).ToList();
        var existingPortionIds = await dbContext.FoodPortions
            .Where(portion => portionIds.Contains(portion.Id))
            .Select(portion => portion.Id)
            .ToListAsync(cancellationToken);
        var existingIds = existingPortionIds.ToHashSet();

        dbContext.FoodPortions.AddRange(portions.Where(portion => !existingIds.Contains(portion.Id)));
    }

    private async Task AddMissingSupplementsAsync(CancellationToken cancellationToken)
    {
        var supplements = CreateSupplements();
        var supplementIds = supplements.Select(supplement => supplement.Id).ToList();
        var existingSupplements = await dbContext.Supplements
            .Where(supplement => supplementIds.Contains(supplement.Id))
            .Select(supplement => new
            {
                supplement.Id,
                Name = supplement.Name.Value,
                supplement.Type
            })
            .ToListAsync(cancellationToken);

        var seedNames = supplements.Select(supplement => supplement.Name.Value).ToList();
        var existingSupplementsByNaturalKey = await dbContext.Supplements
            .Where(supplement => seedNames.Contains(supplement.Name.Value))
            .Select(supplement => new
            {
                supplement.Id,
                Name = supplement.Name.Value,
                supplement.Type
            })
            .ToListAsync(cancellationToken);

        var existingKeys = existingSupplements
            .Concat(existingSupplementsByNaturalKey)
            .SelectMany(supplement => new[]
            {
                SupplementKey(supplement.Id, supplement.Name, supplement.Type),
                SupplementNaturalKey(supplement.Name, supplement.Type)
            })
            .ToHashSet();
        var existingIds = existingSupplements
            .Select(supplement => supplement.Id)
            .ToHashSet();

        dbContext.Supplements.AddRange(supplements.Where(supplement =>
            !existingIds.Contains(supplement.Id) &&
            !existingKeys.Contains(SupplementKey(supplement.Id, supplement.Name.Value, supplement.Type)) &&
            !existingKeys.Contains(SupplementNaturalKey(supplement.Name.Value, supplement.Type))));

        var existingSupplementEntities = await dbContext.Supplements
            .Where(supplement => supplementIds.Contains(supplement.Id))
            .ToListAsync(cancellationToken);

        foreach (var existingSupplement in existingSupplementEntities)
        {
            var seedSupplement = supplements.First(supplement => supplement.Id == existingSupplement.Id);

            if (existingSupplement.Name.Value != seedSupplement.Name.Value)
            {
                existingSupplement.Rename(seedSupplement.Name);
            }

            if (existingSupplement.MacronutrientsPerServing != seedSupplement.MacronutrientsPerServing)
            {
                existingSupplement.UpdateMacronutrients(seedSupplement.MacronutrientsPerServing);
            }
        }
    }

    private async Task AddMissingSubscriptionPlansAsync(CancellationToken cancellationToken)
    {
        var plans = CreateSubscriptionPlans();
        var planIds = plans.Select(plan => plan.Id).ToList();
        var existingPlans = await dbContext.SubscriptionPlans
            .Where(plan => planIds.Contains(plan.Id))
            .Select(plan => new
            {
                plan.Id,
                Name = plan.Name.Value,
                plan.Tier
            })
            .ToListAsync(cancellationToken);

        var seedNames = plans.Select(plan => plan.Name.Value).ToList();
        var existingPlansByNaturalKey = await dbContext.SubscriptionPlans
            .Where(plan => seedNames.Contains(plan.Name.Value))
            .Select(plan => new
            {
                plan.Id,
                Name = plan.Name.Value,
                plan.Tier
            })
            .ToListAsync(cancellationToken);

        var existingKeys = existingPlans
            .Concat(existingPlansByNaturalKey)
            .SelectMany(plan => new[]
            {
                SubscriptionPlanKey(plan.Id, plan.Name, plan.Tier),
                SubscriptionPlanNaturalKey(plan.Name, plan.Tier)
            })
            .ToHashSet();

        dbContext.SubscriptionPlans.AddRange(plans.Where(plan =>
            !existingKeys.Contains(SubscriptionPlanKey(plan.Id, plan.Name.Value, plan.Tier)) &&
            !existingKeys.Contains(SubscriptionPlanNaturalKey(plan.Name.Value, plan.Tier))));
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
                Name("Maçã"),
                FoodCategory.Fruit,
                Nutrition(52m, 0.3m, 14m, 0.2m)),
            Food.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000004"),
                Name("Arroz branco cozido"),
                FoodCategory.Grain,
                Nutrition(130m, 2.7m, 28m, 0.3m)),
            Food.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000005"),
                Name("Feijão carioca cozido"),
                FoodCategory.Legume,
                Nutrition(76m, 4.8m, 13.6m, 0.5m)),
            Food.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000006"),
                Name("Peito de frango sem pele grelhado"),
                FoodCategory.Protein,
                Nutrition(165m, 31m, 0m, 3.6m)),
            Food.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000007"),
                Name("Ovo cozido"),
                FoodCategory.Protein,
                Nutrition(155m, 13m, 1.1m, 11m)),
            Food.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000008"),
                Name("Whey protein genérico"),
                FoodCategory.Supplement,
                Nutrition(400m, 80m, 8m, 6m),
                isSupplement: true),
            Food.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000009"),
                Name("Patinho moído cozido"),
                FoodCategory.Protein,
                Nutrition(219m, 35.9m, 0m, 7.3m)),
            Food.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000010"),
                Name("Batata doce cozida"),
                FoodCategory.Grain,
                Nutrition(77m, 0.6m, 18.4m, 0.1m)),
            Food.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000011"),
                Name("Aveia em flocos"),
                FoodCategory.Grain,
                Nutrition(394m, 13.9m, 66.6m, 8.5m)),
            Food.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000012"),
                Name("Iogurte natural integral"),
                FoodCategory.Dairy,
                Nutrition(63m, 4.1m, 1.9m, 3m)),
            Food.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000013"),
                Name("Tilápia grelhada"),
                FoodCategory.Protein,
                Nutrition(128m, 26.2m, 0m, 2.7m)),
            Food.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000014"),
                Name("Brócolis cozido"),
                FoodCategory.Vegetable,
                Nutrition(25m, 2.1m, 4.4m, 0.5m)),
            Food.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000015"),
                Name("Pão integral"),
                FoodCategory.Grain,
                Nutrition(253m, 9.4m, 49.9m, 3.7m))
        ];
    }

    private static IReadOnlyList<FoodPortion> CreateFoodPortions()
    {
        return
        [
            Portion("11000000-0000-0000-0000-000000000001", "10000000-0000-0000-0000-000000000001", "Pequena", 70m),
            Portion("11000000-0000-0000-0000-000000000002", "10000000-0000-0000-0000-000000000001", "Média", 86m),
            Portion("11000000-0000-0000-0000-000000000003", "10000000-0000-0000-0000-000000000001", "Grande", 118m),
            Portion("11000000-0000-0000-0000-000000000004", "10000000-0000-0000-0000-000000000002", "Pequena", 86m),
            Portion("11000000-0000-0000-0000-000000000005", "10000000-0000-0000-0000-000000000002", "Média", 118m),
            Portion("11000000-0000-0000-0000-000000000006", "10000000-0000-0000-0000-000000000002", "Grande", 136m),
            Portion("11000000-0000-0000-0000-000000000007", "10000000-0000-0000-0000-000000000003", "Pequena", 149m),
            Portion("11000000-0000-0000-0000-000000000008", "10000000-0000-0000-0000-000000000003", "Média", 182m),
            Portion("11000000-0000-0000-0000-000000000009", "10000000-0000-0000-0000-000000000003", "Grande", 223m),
            Portion("11000000-0000-0000-0000-000000000010", "10000000-0000-0000-0000-000000000004", "Pequena", 100m),
            Portion("11000000-0000-0000-0000-000000000011", "10000000-0000-0000-0000-000000000004", "Média", 150m),
            Portion("11000000-0000-0000-0000-000000000012", "10000000-0000-0000-0000-000000000004", "Grande", 200m),
            Portion("11000000-0000-0000-0000-000000000013", "10000000-0000-0000-0000-000000000005", "Pequena", 100m),
            Portion("11000000-0000-0000-0000-000000000014", "10000000-0000-0000-0000-000000000005", "Média", 150m),
            Portion("11000000-0000-0000-0000-000000000015", "10000000-0000-0000-0000-000000000005", "Grande", 200m),
            Portion("11000000-0000-0000-0000-000000000016", "10000000-0000-0000-0000-000000000006", "Pequena", 100m),
            Portion("11000000-0000-0000-0000-000000000017", "10000000-0000-0000-0000-000000000006", "Média", 150m),
            Portion("11000000-0000-0000-0000-000000000018", "10000000-0000-0000-0000-000000000006", "Grande", 200m),
            Portion("11000000-0000-0000-0000-000000000019", "10000000-0000-0000-0000-000000000007", "Pequena", 50m),
            Portion("11000000-0000-0000-0000-000000000020", "10000000-0000-0000-0000-000000000007", "Média", 100m),
            Portion("11000000-0000-0000-0000-000000000021", "10000000-0000-0000-0000-000000000007", "Grande", 150m),
            Portion("11000000-0000-0000-0000-000000000022", "10000000-0000-0000-0000-000000000008", "Pequena", 30m),
            Portion("11000000-0000-0000-0000-000000000023", "10000000-0000-0000-0000-000000000008", "Média", 45m),
            Portion("11000000-0000-0000-0000-000000000024", "10000000-0000-0000-0000-000000000008", "Grande", 60m),
            Portion("11000000-0000-0000-0000-000000000025", "10000000-0000-0000-0000-000000000009", "Pequena", 100m),
            Portion("11000000-0000-0000-0000-000000000026", "10000000-0000-0000-0000-000000000009", "Média", 150m),
            Portion("11000000-0000-0000-0000-000000000027", "10000000-0000-0000-0000-000000000009", "Grande", 200m),
            Portion("11000000-0000-0000-0000-000000000028", "10000000-0000-0000-0000-000000000010", "Pequena", 100m),
            Portion("11000000-0000-0000-0000-000000000029", "10000000-0000-0000-0000-000000000010", "Média", 150m),
            Portion("11000000-0000-0000-0000-000000000030", "10000000-0000-0000-0000-000000000010", "Grande", 200m),
            Portion("11000000-0000-0000-0000-000000000031", "10000000-0000-0000-0000-000000000011", "Pequena", 30m),
            Portion("11000000-0000-0000-0000-000000000032", "10000000-0000-0000-0000-000000000011", "Média", 50m),
            Portion("11000000-0000-0000-0000-000000000033", "10000000-0000-0000-0000-000000000011", "Grande", 80m),
            Portion("11000000-0000-0000-0000-000000000034", "10000000-0000-0000-0000-000000000012", "Pequena", 100m),
            Portion("11000000-0000-0000-0000-000000000035", "10000000-0000-0000-0000-000000000012", "Média", 170m),
            Portion("11000000-0000-0000-0000-000000000036", "10000000-0000-0000-0000-000000000012", "Grande", 250m),
            Portion("11000000-0000-0000-0000-000000000037", "10000000-0000-0000-0000-000000000013", "Pequena", 100m),
            Portion("11000000-0000-0000-0000-000000000038", "10000000-0000-0000-0000-000000000013", "Média", 150m),
            Portion("11000000-0000-0000-0000-000000000039", "10000000-0000-0000-0000-000000000013", "Grande", 200m),
            Portion("11000000-0000-0000-0000-000000000040", "10000000-0000-0000-0000-000000000014", "Pequena", 100m),
            Portion("11000000-0000-0000-0000-000000000041", "10000000-0000-0000-0000-000000000014", "Média", 150m),
            Portion("11000000-0000-0000-0000-000000000042", "10000000-0000-0000-0000-000000000014", "Grande", 200m),
            Portion("11000000-0000-0000-0000-000000000043", "10000000-0000-0000-0000-000000000015", "Pequena", 25m),
            Portion("11000000-0000-0000-0000-000000000044", "10000000-0000-0000-0000-000000000015", "Média", 50m),
            Portion("11000000-0000-0000-0000-000000000045", "10000000-0000-0000-0000-000000000015", "Grande", 75m)
        ];
    }

    private static IReadOnlyList<Supplement> CreateSupplements()
    {
        return
        [
            Supplement.CreateCreatine(
                Guid.Parse("20000000-0000-0000-0000-000000000001"),
                Name("Creatina monohidratada genérica")),
            Supplement.CreateWheyProtein(
                Guid.Parse("20000000-0000-0000-0000-000000000002"),
                Name("Whey protein genérico"),
                new Macronutrients(120m, 24m, 3m, 2m)),
            Supplement.Create(
                Guid.Parse("20000000-0000-0000-0000-000000000003"),
                Name("Pré-treino com cafeína"),
                SupplementType.PreWorkout,
                EmptyMacros()),
            Supplement.Create(
                Guid.Parse("20000000-0000-0000-0000-000000000004"),
                Name("BCAA"),
                SupplementType.Other,
                EmptyMacros()),
            Supplement.Create(
                Guid.Parse("20000000-0000-0000-0000-000000000005"),
                Name("Beta-alanina"),
                SupplementType.Other,
                EmptyMacros()),
            Supplement.Create(
                Guid.Parse("20000000-0000-0000-0000-000000000006"),
                Name("Citrulina malato"),
                SupplementType.Other,
                EmptyMacros()),
            Supplement.Create(
                Guid.Parse("20000000-0000-0000-0000-000000000007"),
                Name("Eletrólitos"),
                SupplementType.Other,
                EmptyMacros()),
            Supplement.Create(
                Guid.Parse("20000000-0000-0000-0000-000000000008"),
                Name("Ômega-3"),
                SupplementType.Other,
                EmptyMacros()),
            Supplement.Create(
                Guid.Parse("20000000-0000-0000-0000-000000000009"),
                Name("Vitamina D"),
                SupplementType.Vitamin,
                EmptyMacros()),
            Supplement.Create(
                Guid.Parse("20000000-0000-0000-0000-000000000010"),
                Name("Magnésio"),
                SupplementType.Mineral,
                EmptyMacros())
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

    private static Macronutrients EmptyMacros()
    {
        return new Macronutrients(0m, 0m, 0m, 0m);
    }

    private static FoodPortion Portion(string id, string foodId, string name, decimal grams)
    {
        return FoodPortion.Create(
            Guid.Parse(id),
            Guid.Parse(foodId),
            Name(name),
            MacroViva.Domain.ValueObjects.Portion.FromGrams(grams));
    }

    private static string FoodKey(Guid id, string name, FoodCategory category, bool isSupplement)
    {
        return $"id:{id}|name:{name}|category:{category}|supplement:{isSupplement}";
    }

    private static string FoodNaturalKey(string name, FoodCategory category, bool isSupplement)
    {
        return $"name:{name}|category:{category}|supplement:{isSupplement}";
    }

    private static string SupplementKey(Guid id, string name, SupplementType type)
    {
        return $"id:{id}|name:{name}|type:{type}";
    }

    private static string SupplementNaturalKey(string name, SupplementType type)
    {
        return $"name:{name}|type:{type}";
    }

    private static string SubscriptionPlanKey(Guid id, string name, SubscriptionTier tier)
    {
        return $"id:{id}|name:{name}|tier:{tier}";
    }

    private static string SubscriptionPlanNaturalKey(string name, SubscriptionTier tier)
    {
        return $"name:{name}|tier:{tier}";
    }
}
