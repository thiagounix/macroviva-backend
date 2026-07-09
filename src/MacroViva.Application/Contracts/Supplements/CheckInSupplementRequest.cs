namespace MacroViva.Application.Contracts.Supplements;

public sealed record CheckInSupplementRequest(Guid SupplementId, DateOnly CheckInDate, decimal Servings);
