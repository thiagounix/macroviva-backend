using MacroViva.Application.Abstractions.Repositories;
using MacroViva.Application.Abstractions.Services;
using MacroViva.Application.Common;
using MacroViva.Application.Contracts.Meals;
using MacroViva.Application.Mapping;

namespace MacroViva.Application.Meals;

public sealed class GetTodayMealsUseCase(
    IMealRepository mealRepository,
    ICurrentUserService currentUserService,
    IClock clock)
{
    public async Task<Result<IReadOnlyList<MealDto>>> ExecuteAsync(CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
        {
            return Result<IReadOnlyList<MealDto>>.Failure(Error.Unauthorized("User.Unauthenticated", "Current user is not authenticated."));
        }

        var meals = await mealRepository.GetByUserAndDateAsync(currentUserService.UserId, clock.Today, cancellationToken);

        return Result<IReadOnlyList<MealDto>>.Success(meals.Select(ApplicationMapper.ToDto).ToList());
    }
}
