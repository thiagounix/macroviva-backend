using MacroViva.Application.Abstractions.Services;
using MacroViva.Application.Contracts.AIAnalysis;
using MacroViva.Application.Contracts.Common;
using MacroViva.Application.Contracts.Foods;
using MacroViva.Application.Contracts.Meals;
using MacroViva.Application.Contracts.Supplements;
using MacroViva.Domain.AIAnalysis;
using MacroViva.Domain.Enums;
using MacroViva.Domain.Meals;
using MacroViva.Domain.Nutrition;
using MacroViva.Domain.Supplements;
using MacroViva.Domain.ValueObjects;

namespace MacroViva.Application.Mapping;

internal static class ApplicationMapper
{
    public static FoodDto ToDto(Food food)
    {
        return new FoodDto(
            food.Id,
            food.Name.Value,
            food.Name.Locale,
            food.Category,
            food.NutritionPer100g.Macronutrients.ToDto(),
            food.IsSupplement,
            food.Portions
                .OrderBy(portion => portion.Portion.Grams)
                .Select(portion => portion.ToDto(food))
                .ToList());
    }

    public static MealDto ToDto(Meal meal)
    {
        return new MealDto(
            meal.Id,
            meal.UserId,
            meal.MealType,
            meal.OccurredAt,
            meal.TotalMacronutrients.ToDto(),
            meal.Items.Select(ToDto).ToList());
    }

    public static SupplementDto ToDto(Supplement supplement)
    {
        var hasStimulantWarning = HasStimulantWarning(supplement);

        return new SupplementDto(
            supplement.Id,
            supplement.Name.Value,
            supplement.Name.Locale,
            supplement.Type,
            supplement.MacronutrientsPerServing.ToDto(),
            supplement.ImpactsMacronutrients,
            SupplementDescription(supplement),
            SupplementSafetyNote(hasStimulantWarning),
            true,
            hasStimulantWarning);
    }

    public static UserSupplementDto ToDto(UserSupplement userSupplement)
    {
        return new UserSupplementDto(
            userSupplement.Id,
            userSupplement.UserId,
            userSupplement.SupplementId,
            userSupplement.CheckInDate,
            userSupplement.Servings,
            userSupplement.MacronutrientImpact.ToDto());
    }

    public static DetectedMealItemDto ToDto(MealVisionDetectedItem item)
    {
        return new DetectedMealItemDto(
            Guid.Empty,
            item.SuggestedFoodName,
            item.Grams,
            item.ConfidenceLevel,
            item.ConfidenceScore,
            item.SuggestedFoodId);
    }

    public static DetectedMealItemDto ToDto(AIAnalysisItem item)
    {
        return new DetectedMealItemDto(
            item.Id,
            item.SuggestedFoodName,
            item.EstimatedPortion.Grams,
            item.ConfidenceLevel,
            item.ConfidenceScore,
            item.SuggestedFoodId);
    }

    public static MacronutrientsDto ToDto(this Macronutrients macronutrients)
    {
        return new MacronutrientsDto(
            macronutrients.Calories,
            macronutrients.ProteinGrams,
            macronutrients.CarbohydrateGrams,
            macronutrients.FatGrams);
    }

    private static FoodPortionDto ToDto(this FoodPortion portion, Food food)
    {
        return new FoodPortionDto(
            portion.Id,
            portion.Name.Value,
            PortionLabel(portion.Name.Value),
            portion.Portion.Grams,
            food.CalculateFor(portion.Portion).ToDto());
    }

    private static string PortionLabel(string name)
    {
        return name.Trim().ToLowerInvariant() switch
        {
            "pequena" => "P",
            "média" => "M",
            "media" => "M",
            "grande" => "G",
            _ => name
        };
    }

    private static string SupplementDescription(Supplement supplement)
    {
        var name = supplement.Name.Value.ToLowerInvariant();

        if (supplement.Type == SupplementType.WheyProtein)
        {
            return "Proteína em pó usada para complementar a ingestão diária de proteína quando a alimentação não é suficiente.";
        }

        if (supplement.Type == SupplementType.Creatine)
        {
            return "Suplemento comum em estratégias nutricionais esportivas, sem substituir alimentação, treino ou orientação profissional.";
        }

        if (supplement.Type == SupplementType.PreWorkout)
        {
            return "Produto usado antes do treino por algumas pessoas, podendo conter cafeína e outros estimulantes.";
        }

        if (name.Contains("bcaa", StringComparison.OrdinalIgnoreCase))
        {
            return "Aminoácidos usados em algumas estratégias nutricionais, conforme avaliação individual.";
        }

        if (name.Contains("beta-alanina", StringComparison.OrdinalIgnoreCase))
        {
            return "Aminoácido usado em algumas estratégias nutricionais esportivas, conforme orientação profissional.";
        }

        if (name.Contains("citrulina", StringComparison.OrdinalIgnoreCase))
        {
            return "Suplemento usado em algumas estratégias nutricionais esportivas, conforme avaliação individual.";
        }

        if (name.Contains("eletrólitos", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("eletrolitos", StringComparison.OrdinalIgnoreCase))
        {
            return "Mistura de minerais usada para apoiar o registro de reposição nutricional em contextos individuais.";
        }

        if (name.Contains("ômega-3", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("omega-3", StringComparison.OrdinalIgnoreCase))
        {
            return "Fonte de ácidos graxos usada como complemento nutricional quando indicada por profissional.";
        }

        if (supplement.Type == SupplementType.Vitamin)
        {
            return "Vitamina usada como complemento nutricional quando há indicação profissional.";
        }

        if (supplement.Type == SupplementType.Mineral)
        {
            return "Mineral usado como complemento nutricional quando há indicação profissional.";
        }

        return "Suplemento usado em estratégias nutricionais individuais, conforme orientação profissional.";
    }

    private static string SupplementSafetyNote(bool hasStimulantWarning)
    {
        const string generalSafetyNote =
            "Suplemento não substitui alimentação equilibrada nem orientação médica/nutricional.";

        if (!hasStimulantWarning)
        {
            return generalSafetyNote;
        }

        return "Pode conter estimulantes, como cafeína. Use apenas com orientação profissional, especialmente em caso de sensibilidade, ansiedade, hipertensão, problemas cardíacos ou uso de medicamentos. " +
            generalSafetyNote;
    }

    private static bool HasStimulantWarning(Supplement supplement)
    {
        return supplement.Type == SupplementType.PreWorkout ||
            supplement.Name.Value.Contains("cafeína", StringComparison.OrdinalIgnoreCase) ||
            supplement.Name.Value.Contains("cafeina", StringComparison.OrdinalIgnoreCase);
    }

    private static MealItemDto ToDto(MealItem item)
    {
        return new MealItemDto(
            item.Id,
            item.FoodId,
            item.FoodNameSnapshot.Value,
            item.Portion.Grams,
            item.MacronutrientsSnapshot.ToDto());
    }
}
