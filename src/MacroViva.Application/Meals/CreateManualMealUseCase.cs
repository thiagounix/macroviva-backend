using MacroViva.Application.Abstractions.Repositories;
using MacroViva.Application.Abstractions.Services;
using MacroViva.Application.Common;
using MacroViva.Application.Contracts.Meals;
using MacroViva.Application.Mapping;
using MacroViva.Domain.Common;
using MacroViva.Domain.Meals;
using MacroViva.Domain.ValueObjects;

namespace MacroViva.Application.Meals;

public sealed class CreateManualMealUseCase(
    IFoodRepository foodRepository,
    IMealRepository mealRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork)
{
    public async Task<Result<MealDto>> ExecuteAsync(CreateMealRequest request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
        {
            return Result<MealDto>.Failure(Error.Unauthorized("User.Unauthenticated", "Current user is not authenticated."));
        }

        if (request is null)
        {
            return Result<MealDto>.Failure(Error.Validation("Meal.InvalidRequest", "Meal request is required."));
        }

        if (request.Items.Count == 0)
        {
            return Result<MealDto>.Failure(Error.Validation("Meal.EmptyItems", "Meal must contain at least one item."));
        }

        try
        {
            var meal = Meal.Create(Guid.NewGuid(), currentUserService.UserId, request.MealType, request.OccurredAt);

            foreach (var item in request.Items)
            {
                if (item.FoodId == Guid.Empty)
                {
                    return Result<MealDto>.Failure(Error.Validation("MealItem.InvalidFoodId", "Food id is required."));
                }

                if (item.Grams <= 0)
                {
                    return Result<MealDto>.Failure(Error.Validation("MealItem.InvalidGrams", "Item grams must be greater than zero."));
                }

                var food = await foodRepository.GetByIdAsync(item.FoodId, cancellationToken);

                if (food is null)
                {
                    return Result<MealDto>.Failure(Error.NotFound("Food.NotFound", $"Food '{item.FoodId}' was not found."));
                }

                meal.AddFood(Guid.NewGuid(), food, Portion.FromGrams(item.Grams));
            }

            await mealRepository.AddAsync(meal, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<MealDto>.Success(ApplicationMapper.ToDto(meal));
        }
        catch (DomainException exception)
        {
            return Result<MealDto>.Failure(Error.Validation("Meal.DomainRuleViolation", exception.Message));
        }
    }
}
