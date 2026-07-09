namespace MacroViva.Application.Contracts.AIAnalysis;

public sealed record AnalyzeMealPhotoResponse(
    Guid AnalysisId,
    string FileReference,
    IReadOnlyList<DetectedMealItemDto> Items);
