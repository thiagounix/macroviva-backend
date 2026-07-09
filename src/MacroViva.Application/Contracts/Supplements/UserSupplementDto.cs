using MacroViva.Application.Contracts.Common;

namespace MacroViva.Application.Contracts.Supplements;

public sealed record UserSupplementDto(
    Guid Id,
    Guid UserId,
    Guid SupplementId,
    DateOnly CheckInDate,
    decimal Servings,
    MacronutrientsDto MacronutrientImpact);
