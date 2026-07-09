using MacroViva.Domain.Enums;

namespace MacroViva.Api.Contracts.AIAnalysis;

public sealed class AnalyzeMealPhotoHttpRequest
{
    public IFormFile? File { get; init; }

    public MealType? MealType { get; init; }
}
