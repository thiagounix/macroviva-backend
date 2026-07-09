using MacroViva.Api.Common;
using MacroViva.Api.Contracts.AIAnalysis;
using MacroViva.Application.AIAnalysis;
using MacroViva.Application.Contracts.AIAnalysis;
using MacroViva.Application.Contracts.Meals;
using MacroViva.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MacroViva.Api.Controllers;

[ApiController]
[Route("api/ai/meal-photo")]
public sealed class AIAnalysisController(
    AnalyzeMealPhotoUseCase analyzeMealPhotoUseCase,
    ConfirmMealAnalysisUseCase confirmMealAnalysisUseCase) : ControllerBase
{
    [HttpPost("analyze")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(AnalyzeMealPhotoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Analyze(
        [FromForm] AnalyzeMealPhotoHttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        var file = httpRequest.File;

        if (file is null || file.Length == 0)
        {
            return BadRequest(new { Code = "MealPhoto.EmptyFile", Message = "File is required." });
        }

        await using var stream = file.OpenReadStream();
        var request = new AnalyzeMealPhotoRequest(stream, file.FileName, file.ContentType);
        var result = await analyzeMealPhotoUseCase.ExecuteAsync(request, cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("{analysisId:guid}/confirm")]
    [ProducesResponseType(typeof(MealDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Confirm(
        [FromRoute] Guid analysisId,
        [FromBody] ConfirmMealAnalysisHttpRequest request,
        CancellationToken cancellationToken)
    {
        var applicationRequest = new ConfirmMealAnalysisRequest(
            analysisId,
            request.MealType,
            request.OccurredAt,
            request.Items);

        var result = await confirmMealAnalysisUseCase.ExecuteAsync(applicationRequest, cancellationToken);

        return this.ToActionResult(result);
    }
}
