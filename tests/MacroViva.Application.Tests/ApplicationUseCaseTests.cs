using MacroViva.Application.Abstractions.Services;
using MacroViva.Application.Common;
using MacroViva.Application.AIAnalysis;
using MacroViva.Application.Contracts.AIAnalysis;
using MacroViva.Application.Contracts.Meals;
using MacroViva.Application.Contracts.Supplements;
using MacroViva.Application.Foods;
using MacroViva.Application.Meals;
using MacroViva.Application.Supplements;
using MacroViva.Domain.AIAnalysis;
using MacroViva.Domain.Enums;
using MacroViva.Domain.Nutrition;
using MacroViva.Domain.Supplements;
using MacroViva.Domain.ValueObjects;
using MealImageAnalysis = MacroViva.Domain.AIAnalysis.AIAnalysis;

namespace MacroViva.Application.Tests;

public sealed class ApplicationUseCaseTests
{
    private static readonly Guid UserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly DateTimeOffset Now = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
    private static readonly DateOnly Today = new(2026, 1, 1);

    [Fact]
    public async Task CreateManualMealShouldCreateMealAndTotalMacros()
    {
        var foodRepository = new FakeFoodRepository();
        var chicken = CreateFood("Chicken", FoodCategory.Protein, new Macronutrients(165m, 31m, 0m, 3.6m));
        var rice = CreateFood("Rice", FoodCategory.Grain, new Macronutrients(130m, 2.7m, 28m, 0.3m));
        foodRepository.Add(chicken);
        foodRepository.Add(rice);
        var mealRepository = new FakeMealRepository();
        var unitOfWork = new FakeUnitOfWork();
        var useCase = new CreateManualMealUseCase(
            foodRepository,
            mealRepository,
            new FakeCurrentUserService(UserId),
            unitOfWork);

        var result = await useCase.ExecuteAsync(
            new CreateMealRequest(
                MealType.Lunch,
                Now,
                [
                    new CreateMealItemRequest(chicken.Id, 200m),
                    new CreateMealItemRequest(rice.Id, 150m)
                ]),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(mealRepository.AddedMeals);
        Assert.Equal(525m, result.Value!.TotalMacronutrients.Calories);
        Assert.Equal(66.05m, result.Value.TotalMacronutrients.ProteinGrams);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task CreateManualMealShouldReturnFailureWhenFoodDoesNotExist()
    {
        var useCase = new CreateManualMealUseCase(
            new FakeFoodRepository(),
            new FakeMealRepository(),
            new FakeCurrentUserService(UserId),
            new FakeUnitOfWork());

        var result = await useCase.ExecuteAsync(
            new CreateMealRequest(
                MealType.Lunch,
                Now,
                [new CreateMealItemRequest(Guid.NewGuid(), 100m)]),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.Error!.Type);
    }

    [Fact]
    public async Task SearchFoodsShouldReturnCompatibleFoods()
    {
        var foodRepository = new FakeFoodRepository();
        foodRepository.Add(CreateFood("White rice", FoodCategory.Grain, new Macronutrients(130m, 2.7m, 28m, 0.3m)));
        foodRepository.Add(CreateFood("Chicken breast", FoodCategory.Protein, new Macronutrients(165m, 31m, 0m, 3.6m)));
        var useCase = new SearchFoodsUseCase(foodRepository);

        var result = await useCase.ExecuteAsync("rice", CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Foods);
        Assert.Equal("White rice", result.Value.Foods[0].Name);
    }

    [Fact]
    public async Task AnalyzeMealPhotoShouldReturnDetectedItemsUsingFakeAnalyzer()
    {
        var analysisRepository = new FakeAIAnalysisRepository();
        var useCase = new AnalyzeMealPhotoUseCase(
            analysisRepository,
            new FakeFileStorageService(),
            new FakeMealVisionAnalyzer(
                [
                    new("Rice", 120m, ConfidenceLevel.High, 0.91m, Guid.NewGuid())
                ]),
            new FakeCurrentUserService(UserId),
            new FakeClock(Now, Today),
            new FakeUnitOfWork());

        await using var stream = new MemoryStream([1, 2, 3]);
        var result = await useCase.ExecuteAsync(
            new AnalyzeMealPhotoRequest(stream, "meal.jpg", "image/jpeg"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value!.AnalysisId);
        Assert.Single(result.Value.Items);
        Assert.NotEqual(Guid.Empty, result.Value.Items[0].AnalysisItemId);
        Assert.Single(analysisRepository.Analyses);
    }

    [Fact]
    public async Task ConfirmMealAnalysisShouldCreateMealFromCompletedAnalysisAndConfirmedItems()
    {
        var rice = CreateFood("Rice", FoodCategory.Grain, new Macronutrients(130m, 2.7m, 28m, 0.3m));
        var foodRepository = new FakeFoodRepository();
        foodRepository.Add(rice);
        var analysis = CreateCompletedAnalysis();
        var analysisRepository = new FakeAIAnalysisRepository();
        analysisRepository.AddExisting(analysis);
        var mealRepository = new FakeMealRepository();
        var unitOfWork = new FakeUnitOfWork();
        var useCase = new ConfirmMealAnalysisUseCase(
            analysisRepository,
            foodRepository,
            mealRepository,
            new FakeCurrentUserService(UserId),
            new FakeClock(Now.AddMinutes(2), Today),
            unitOfWork);

        var result = await useCase.ExecuteAsync(
            new ConfirmMealAnalysisRequest(
                analysis.Id,
                MealType.Lunch,
                Now,
                [new ConfirmMealAnalysisItemRequest(analysis.Items.First().Id, rice.Id, 150m)]),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(mealRepository.AddedMeals);
        Assert.Equal(195m, result.Value!.TotalMacronutrients.Calories);
        Assert.Equal(AIAnalysisStatus.Confirmed, analysis.Status);
        Assert.Equal(1, analysisRepository.UpdateCalls);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task ConfirmMealAnalysisShouldReturnFailureWhenAnalysisIsPending()
    {
        var analysis = MealImageAnalysis.StartPending(Guid.NewGuid(), UserId, Now);
        var analysisRepository = new FakeAIAnalysisRepository();
        analysisRepository.AddExisting(analysis);
        var useCase = new ConfirmMealAnalysisUseCase(
            analysisRepository,
            new FakeFoodRepository(),
            new FakeMealRepository(),
            new FakeCurrentUserService(UserId),
            new FakeClock(Now.AddMinutes(2), Today),
            new FakeUnitOfWork());

        var result = await useCase.ExecuteAsync(
            new ConfirmMealAnalysisRequest(
                analysis.Id,
                MealType.Lunch,
                Now,
                [new ConfirmMealAnalysisItemRequest(Guid.NewGuid(), Guid.NewGuid(), 100m)]),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Conflict, result.Error!.Type);
    }

    [Fact]
    public async Task ConfirmMealAnalysisShouldReturnFailureWhenSelectedFoodDoesNotExist()
    {
        var analysis = CreateCompletedAnalysis();
        var analysisRepository = new FakeAIAnalysisRepository();
        analysisRepository.AddExisting(analysis);
        var useCase = new ConfirmMealAnalysisUseCase(
            analysisRepository,
            new FakeFoodRepository(),
            new FakeMealRepository(),
            new FakeCurrentUserService(UserId),
            new FakeClock(Now.AddMinutes(2), Today),
            new FakeUnitOfWork());

        var result = await useCase.ExecuteAsync(
            new ConfirmMealAnalysisRequest(
                analysis.Id,
                MealType.Lunch,
                Now,
                [new ConfirmMealAnalysisItemRequest(analysis.Items.First().Id, Guid.NewGuid(), 150m)]),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.Error!.Type);
    }

    [Fact]
    public async Task CheckInUserSupplementShouldRegisterCreatineForTheDay()
    {
        var creatine = Supplement.CreateCreatine(
            Guid.NewGuid(),
            new LocalizedName("Creatine", LocaleCode.EnUs));
        var supplementRepository = new FakeSupplementRepository();
        supplementRepository.Add(creatine);
        var userSupplementRepository = new FakeUserSupplementRepository();
        var unitOfWork = new FakeUnitOfWork();
        var useCase = new CheckInUserSupplementUseCase(
            supplementRepository,
            userSupplementRepository,
            new FakeCurrentUserService(UserId),
            unitOfWork);

        var result = await useCase.ExecuteAsync(
            new CheckInSupplementRequest(creatine.Id, Today, 1m),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(userSupplementRepository.Added);
        Assert.Equal(Macronutrients.Zero.Calories, result.Value!.MacronutrientImpact.Calories);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task CheckInUserSupplementShouldReturnFailureWhenSupplementDoesNotExist()
    {
        var useCase = new CheckInUserSupplementUseCase(
            new FakeSupplementRepository(),
            new FakeUserSupplementRepository(),
            new FakeCurrentUserService(UserId),
            new FakeUnitOfWork());

        var result = await useCase.ExecuteAsync(
            new CheckInSupplementRequest(Guid.NewGuid(), Today, 1m),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.Error!.Type);
    }

    private static Food CreateFood(string name, FoodCategory category, Macronutrients macrosPer100g)
    {
        return Food.Create(
            Guid.NewGuid(),
            new LocalizedName(name, LocaleCode.EnUs),
            category,
            new NutritionPer100g(macrosPer100g));
    }

    private static MealImageAnalysis CreateCompletedAnalysis()
    {
        var item = AIAnalysisItem.Create(
            Guid.NewGuid(),
            "Rice",
            Portion.FromGrams(120m),
            ConfidenceLevel.High,
            0.91m,
            Guid.NewGuid());

        return MealImageAnalysis.CreateCompleted(Guid.NewGuid(), UserId, Now, Now.AddMinutes(1), [item]);
    }
}
