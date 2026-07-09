namespace MacroViva.Application.Contracts.AIAnalysis;

public sealed record AnalyzeMealPhotoRequest(Stream Content, string FileName, string ContentType);
