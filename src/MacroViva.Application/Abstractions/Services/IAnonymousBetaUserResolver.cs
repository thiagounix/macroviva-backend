namespace MacroViva.Application.Abstractions.Services;

public interface IAnonymousBetaUserResolver
{
    Task<Guid> ResolveOrCreateAsync(Guid installationId, CancellationToken cancellationToken);
}
