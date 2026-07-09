using MacroViva.Api.Common;
using MacroViva.Application.Contracts.Foods;
using MacroViva.Application.Foods;
using Microsoft.AspNetCore.Mvc;

namespace MacroViva.Api.Controllers;

[ApiController]
[Route("api/foods")]
public sealed class FoodsController(
    SearchFoodsUseCase searchFoodsUseCase,
    GetFoodByIdUseCase getFoodByIdUseCase) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(FoodSearchResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string? search, CancellationToken cancellationToken)
    {
        var result = await searchFoodsUseCase.ExecuteAsync(search, cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FoodDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await getFoodByIdUseCase.ExecuteAsync(id, cancellationToken);

        return this.ToActionResult(result);
    }
}
