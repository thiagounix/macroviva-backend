namespace MacroViva.Application.Abstractions.Services;

public interface IFileStorageService
{
    Task<FileStorageResult> SaveAsync(FileStorageRequest request, CancellationToken cancellationToken);
}
