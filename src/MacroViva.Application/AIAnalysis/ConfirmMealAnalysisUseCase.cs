using MacroViva.Application.Abstractions.Repositories;
using MacroViva.Application.Abstractions.Services;
using MacroViva.Application.Common;
using MacroViva.Application.Contracts.AIAnalysis;
using MacroViva.Application.Contracts.Meals;
using MacroViva.Application.Mapping;
using MacroViva.Domain.Common;
using MacroViva.Domain.Enums;
using MacroViva.Domain.Meals;
using MacroViva.Domain.ValueObjects;

namespace MacroViva.Application.AIAnalysis;

public sealed class ConfirmMealAnalysisUseCase(
    IAIAnalysisRepository analysisRepository,
    IFoodRepository foodRepository,
    IMealRepository mealRepository,
    ICurrentUserService currentUserService,
    IClock clock,
    IUnitOfWork unitOfWork)
{
    public async Task<Result<MealDto>> ExecuteAsync(ConfirmMealAnalysisRequest request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
        {
            return Result<MealDto>.Failure(Error.Unauthorized("User.Unauthenticated", "Current user is not authenticated."));
        }

        if (request is null)
        {
            return Result<MealDto>.Failure(Error.Validation("AIAnalysis.InvalidRequest", "Confirmation request is required."));
        }

        if (request.Items.Count == 0)
        {
            return Result<MealDto>.Failure(Error.Validation("AIAnalysis.EmptyConfirmedItems", "At least one confirmed item is required."));
        }

        var analysis = await analysisRepository.GetByIdAsync(request.AnalysisId, cancellationToken);

        if (analysis is null)
        {
            return Result<MealDto>.Failure(Error.NotFound("AIAnalysis.NotFound", "AI analysis was not found."));
        }

        if (analysis.UserId != currentUserService.UserId)
        {
            return Result<MealDto>.Failure(Error.NotFound("AIAnalysis.NotFound", "AI analysis was not found."));
        }

        if (analysis.Status != AIAnalysisStatus.Completed)
        {
            return Result<MealDto>.Failure(Error.Conflict("AIAnalysis.InvalidStatus", "Only completed AI analyses can be confirmed."));
        }

        try
        {
            var meal = Meal.Create(Guid.NewGuid(), currentUserService.UserId, request.MealType, request.OccurredAt);

            foreach (var item in request.Items)
            {
                if (item.SelectedFoodId == Guid.Empty)
                {
                    return Result<MealDto>.Failure(Error.Validation("MealItem.InvalidFoodId", "Selected food id is required."));
                }

                if (item.Grams <= 0)
                {
                    return Result<MealDto>.Failure(Error.Validation("MealItem.InvalidGrams", "Item grams must be greater than zero."));
                }

                var selectedFood = await foodRepository.GetByIdAsync(item.SelectedFoodId, cancellationToken);

                if (selectedFood is null)
                {
                    return Result<MealDto>.Failure(Error.NotFound("Food.NotFound", $"Food '{item.SelectedFoodId}' was not found."));
                }

                meal.AddFood(Guid.NewGuid(), selectedFood, Portion.FromGrams(item.Grams));
            }

            analysis.Confirm(clock.UtcNow);
            analysisRepository.Update(analysis);

            await mealRepository.AddAsync(meal, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<MealDto>.Success(ApplicationMapper.ToDto(meal));
        }
        catch (DomainException exception)
        {
            return Result<MealDto>.Failure(Error.Validation("AIAnalysis.DomainRuleViolation", exception.Message));
        }
    }
}
