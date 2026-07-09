using MacroViva.Application.Abstractions.Repositories;
using MacroViva.Application.Abstractions.Services;
using MacroViva.Application.Common;
using MacroViva.Application.Contracts.AIAnalysis;
using MacroViva.Application.Mapping;
using MacroViva.Domain.AIAnalysis;
using MacroViva.Domain.Common;
using MacroViva.Domain.ValueObjects;
using MealImageAnalysis = MacroViva.Domain.AIAnalysis.AIAnalysis;

namespace MacroViva.Application.AIAnalysis;

public sealed class AnalyzeMealPhotoUseCase(
    IAIAnalysisRepository analysisRepository,
    IFileStorageService fileStorageService,
    IMealVisionAnalyzer mealVisionAnalyzer,
    ICurrentUserService currentUserService,
    IClock clock,
    IUnitOfWork unitOfWork)
{
    public async Task<Result<AnalyzeMealPhotoResponse>> ExecuteAsync(AnalyzeMealPhotoRequest request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
        {
            return Result<AnalyzeMealPhotoResponse>.Failure(Error.Unauthorized("User.Unauthenticated", "Current user is not authenticated."));
        }

        if (request is null)
        {
            return Result<AnalyzeMealPhotoResponse>.Failure(Error.Validation("MealPhoto.InvalidRequest", "Meal photo request is required."));
        }

        if (request.Content is null || !request.Content.CanRead)
        {
            return Result<AnalyzeMealPhotoResponse>.Failure(Error.Validation("MealPhoto.InvalidContent", "Meal photo content must be readable."));
        }

        if (string.IsNullOrWhiteSpace(request.FileName))
        {
            return Result<AnalyzeMealPhotoResponse>.Failure(Error.Validation("MealPhoto.InvalidFileName", "Meal photo file name is required."));
        }

        if (string.IsNullOrWhiteSpace(request.ContentType))
        {
            return Result<AnalyzeMealPhotoResponse>.Failure(Error.Validation("MealPhoto.InvalidContentType", "Meal photo content type is required."));
        }

        try
        {
            var storedFile = await fileStorageService.SaveAsync(
                new FileStorageRequest(request.Content, request.FileName, request.ContentType),
                cancellationToken);

            var visionResult = await mealVisionAnalyzer.AnalyzeAsync(
                new MealVisionAnalysisRequest(
                    currentUserService.UserId,
                    storedFile.FileReference,
                    storedFile.FileName,
                    storedFile.ContentType),
                cancellationToken);

            if (visionResult.Items is null || visionResult.Items.Count == 0)
            {
                return Result<AnalyzeMealPhotoResponse>.Failure(Error.Validation("AIAnalysis.EmptyDetectedItems", "AI analysis must return at least one detected item."));
            }

            var detectedItems = visionResult.Items
                .Select(item => AIAnalysisItem.Create(
                    Guid.NewGuid(),
                    item.SuggestedFoodName,
                    Portion.FromGrams(item.Grams),
                    item.ConfidenceLevel,
                    item.ConfidenceScore,
                    item.SuggestedFoodId))
                .ToList();

            var now = clock.UtcNow;
            var analysis = MealImageAnalysis.CreateCompleted(
                Guid.NewGuid(),
                currentUserService.UserId,
                now,
                now,
                detectedItems);

            await analysisRepository.AddAsync(analysis, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new AnalyzeMealPhotoResponse(
                analysis.Id,
                storedFile.FileReference,
                visionResult.Items.Select(ApplicationMapper.ToDto).ToList());

            return Result<AnalyzeMealPhotoResponse>.Success(response);
        }
        catch (DomainException exception)
        {
            return Result<AnalyzeMealPhotoResponse>.Failure(Error.Validation("AIAnalysis.DomainRuleViolation", exception.Message));
        }
    }
}
