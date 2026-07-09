using MacroViva.Application.Contracts.Common;
using MacroViva.Domain.Enums;

namespace MacroViva.Application.Contracts.Supplements;

public sealed record SupplementDto(
    Guid Id,
    string Name,
    LocaleCode Locale,
    SupplementType Type,
    MacronutrientsDto MacronutrientsPerServing,
    bool ImpactsMacronutrients);
