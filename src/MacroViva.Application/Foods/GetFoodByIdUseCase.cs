using MacroViva.Application.Abstractions.Repositories;
using MacroViva.Application.Common;
using MacroViva.Application.Contracts.Foods;
using MacroViva.Application.Mapping;

namespace MacroViva.Application.Foods;

public sealed class GetFoodByIdUseCase(IFoodRepository foodRepository)
{
    public async Task<Result<FoodDto>> ExecuteAsync(Guid foodId, CancellationToken cancellationToken)
    {
        if (foodId == Guid.Empty)
        {
            return Result<FoodDto>.Failure(Error.Validation("Food.InvalidId", "Food id is required."));
        }

        var food = await foodRepository.GetByIdAsync(foodId, cancellationToken);

        return food is null
            ? Result<FoodDto>.Failure(Error.NotFound("Food.NotFound", "Food was not found."))
            : Result<FoodDto>.Success(ApplicationMapper.ToDto(food));
    }
}
