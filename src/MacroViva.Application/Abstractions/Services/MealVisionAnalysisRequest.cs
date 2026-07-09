namespace MacroViva.Application.Abstractions.Services;

public sealed record MealVisionAnalysisRequest(
    Guid UserId,
    string FileReference,
    string FileName,
    string ContentType);
