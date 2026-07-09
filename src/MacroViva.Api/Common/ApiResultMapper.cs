using MacroViva.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace MacroViva.Api.Common;

public static class ApiResultMapper
{
    public static IActionResult ToActionResult<T>(this ControllerBase controller, Result<T> result)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(result.Value);
        }

        return controller.ToErrorResult(result.Error);
    }

    public static IActionResult ToCreatedResult<T>(this ControllerBase controller, Result<T> result, string? location = null)
    {
        if (result.IsSuccess)
        {
            return string.IsNullOrWhiteSpace(location)
                ? controller.StatusCode(StatusCodes.Status201Created, result.Value)
                : controller.Created(location, result.Value);
        }

        return controller.ToErrorResult(result.Error);
    }

    private static IActionResult ToErrorResult(this ControllerBase controller, Error? error)
    {
        if (error is null)
        {
            return controller.Problem(
                title: "Unexpected error",
                detail: "An unexpected error occurred.",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        var payload = new
        {
            error.Code,
            error.Message
        };

        return error.Type switch
        {
            ErrorType.Validation => controller.BadRequest(payload),
            ErrorType.NotFound => controller.NotFound(payload),
            ErrorType.Unauthorized => controller.Unauthorized(payload),
            ErrorType.Conflict => controller.Conflict(payload),
            _ => controller.Problem(
                title: error.Code,
                detail: error.Message,
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}
