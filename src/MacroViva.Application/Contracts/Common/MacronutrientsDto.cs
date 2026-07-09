namespace MacroViva.Application.Contracts.Common;

public sealed record MacronutrientsDto(
    decimal Calories,
    decimal ProteinGrams,
    decimal CarbohydrateGrams,
    decimal FatGrams);
