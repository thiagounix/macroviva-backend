namespace MacroViva.Application.Abstractions.Services;

public sealed record FileStorageRequest(Stream Content, string FileName, string ContentType);
