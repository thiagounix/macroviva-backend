using MacroViva.Application.Abstractions.Services;
using Microsoft.Extensions.Configuration;

namespace MacroViva.Infrastructure.Services;

public sealed class LocalFileStorageService(IConfiguration configuration) : IFileStorageService
{
    public async Task<FileStorageResult> SaveAsync(FileStorageRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var rootPath = configuration["LocalFileStorage:RootPath"];

        if (string.IsNullOrWhiteSpace(rootPath))
        {
            rootPath = Path.Combine(AppContext.BaseDirectory, "local-storage", "meal-photos");
        }

        Directory.CreateDirectory(rootPath);

        var extension = Path.GetExtension(request.FileName);

        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".bin";
        }

        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(rootPath, storedFileName);

        await using var fileStream = File.Create(fullPath);
        await request.Content.CopyToAsync(fileStream, cancellationToken);

        return new FileStorageResult(fullPath, request.FileName, request.ContentType);
    }
}
