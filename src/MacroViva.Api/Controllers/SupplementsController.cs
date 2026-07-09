using MacroViva.Api.Common;
using MacroViva.Application.Contracts.Supplements;
using MacroViva.Application.Supplements;
using Microsoft.AspNetCore.Mvc;

namespace MacroViva.Api.Controllers;

[ApiController]
public sealed class SupplementsController(
    GetSupplementsUseCase getSupplementsUseCase,
    CheckInUserSupplementUseCase checkInUserSupplementUseCase) : ControllerBase
{
    [HttpGet("api/supplements")]
    [ProducesResponseType(typeof(IReadOnlyList<SupplementDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await getSupplementsUseCase.ExecuteAsync(cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("api/user-supplements/check-in")]
    [ProducesResponseType(typeof(UserSupplementDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckIn(
        [FromBody] CheckInSupplementRequest request,
        CancellationToken cancellationToken)
    {
        var result = await checkInUserSupplementUseCase.ExecuteAsync(request, cancellationToken);

        return this.ToActionResult(result);
    }
}
