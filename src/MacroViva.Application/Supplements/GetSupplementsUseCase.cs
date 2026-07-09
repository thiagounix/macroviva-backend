using MacroViva.Application.Abstractions.Repositories;
using MacroViva.Application.Common;
using MacroViva.Application.Contracts.Supplements;
using MacroViva.Application.Mapping;

namespace MacroViva.Application.Supplements;

public sealed class GetSupplementsUseCase(ISupplementRepository supplementRepository)
{
    public async Task<Result<IReadOnlyList<SupplementDto>>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var supplements = await supplementRepository.GetAllAsync(cancellationToken);

        return Result<IReadOnlyList<SupplementDto>>.Success(supplements.Select(ApplicationMapper.ToDto).ToList());
    }
}
