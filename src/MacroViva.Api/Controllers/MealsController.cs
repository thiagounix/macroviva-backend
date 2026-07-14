using MacroViva.Api.Common;
using MacroViva.Application.Contracts.Meals;
using MacroViva.Application.Meals;
using Microsoft.AspNetCore.Mvc;

namespace MacroViva.Api.Controllers;

[ApiController]
[Route("api/meals")]
[RequiresTesterIdentity]
public sealed class MealsController(
    CreateManualMealUseCase createManualMealUseCase,
    GetTodayMealsUseCase getTodayMealsUseCase) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(MealDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMealRequest request, CancellationToken cancellationToken)
    {
        var result = await createManualMealUseCase.ExecuteAsync(request, cancellationToken);

        return this.ToCreatedResult(result);
    }

    [HttpGet("today")]
    [ProducesResponseType(typeof(IReadOnlyList<MealDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetToday(CancellationToken cancellationToken)
    {
        var result = await getTodayMealsUseCase.ExecuteAsync(cancellationToken);

        return this.ToActionResult(result);
    }
}
