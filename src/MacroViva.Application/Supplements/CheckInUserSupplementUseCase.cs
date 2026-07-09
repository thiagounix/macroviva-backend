using MacroViva.Application.Abstractions.Repositories;
using MacroViva.Application.Abstractions.Services;
using MacroViva.Application.Common;
using MacroViva.Application.Contracts.Supplements;
using MacroViva.Application.Mapping;
using MacroViva.Domain.Common;
using MacroViva.Domain.Supplements;

namespace MacroViva.Application.Supplements;

public sealed class CheckInUserSupplementUseCase(
    ISupplementRepository supplementRepository,
    IUserSupplementRepository userSupplementRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork)
{
    public async Task<Result<UserSupplementDto>> ExecuteAsync(CheckInSupplementRequest request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
        {
            return Result<UserSupplementDto>.Failure(Error.Unauthorized("User.Unauthenticated", "Current user is not authenticated."));
        }

        if (request is null)
        {
            return Result<UserSupplementDto>.Failure(Error.Validation("Supplement.InvalidRequest", "Supplement check-in request is required."));
        }

        if (request.SupplementId == Guid.Empty)
        {
            return Result<UserSupplementDto>.Failure(Error.Validation("Supplement.InvalidId", "Supplement id is required."));
        }

        if (request.Servings <= 0)
        {
            return Result<UserSupplementDto>.Failure(Error.Validation("Supplement.InvalidServings", "Servings must be greater than zero."));
        }

        var supplement = await supplementRepository.GetByIdAsync(request.SupplementId, cancellationToken);

        if (supplement is null)
        {
            return Result<UserSupplementDto>.Failure(Error.NotFound("Supplement.NotFound", "Supplement was not found."));
        }

        try
        {
            var checkIn = UserSupplement.CheckIn(
                Guid.NewGuid(),
                currentUserService.UserId,
                supplement,
                request.CheckInDate,
                request.Servings);

            await userSupplementRepository.AddAsync(checkIn, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<UserSupplementDto>.Success(ApplicationMapper.ToDto(checkIn));
        }
        catch (DomainException exception)
        {
            return Result<UserSupplementDto>.Failure(Error.Validation("Supplement.DomainRuleViolation", exception.Message));
        }
    }
}
