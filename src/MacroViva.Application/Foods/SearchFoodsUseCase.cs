using MacroViva.Application.Abstractions.Repositories;
using MacroViva.Application.Common;
using MacroViva.Application.Contracts.Foods;
using MacroViva.Application.Mapping;

namespace MacroViva.Application.Foods;

public sealed class SearchFoodsUseCase(IFoodRepository foodRepository)
{
    public async Task<Result<FoodSearchResponse>> ExecuteAsync(string? search, CancellationToken cancellationToken)
    {
        var foods = await foodRepository.SearchAsync(search, cancellationToken);
        var response = new FoodSearchResponse(foods.Select(ApplicationMapper.ToDto).ToList());

        return Result<FoodSearchResponse>.Success(response);
    }
}
