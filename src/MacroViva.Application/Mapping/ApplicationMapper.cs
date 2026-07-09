using MacroViva.Application.Abstractions.Services;
using MacroViva.Application.Contracts.AIAnalysis;
using MacroViva.Application.Contracts.Common;
using MacroViva.Application.Contracts.Foods;
using MacroViva.Application.Contracts.Meals;
using MacroViva.Application.Contracts.Supplements;
using MacroViva.Domain.AIAnalysis;
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
            food.IsSupplement);
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
        return new SupplementDto(
            supplement.Id,
            supplement.Name.Value,
            supplement.Name.Locale,
            supplement.Type,
            supplement.MacronutrientsPerServing.ToDto(),
            supplement.ImpactsMacronutrients);
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
