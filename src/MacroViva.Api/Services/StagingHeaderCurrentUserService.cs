using MacroViva.Application.Abstractions.Services;

namespace MacroViva.Api.Services;

public sealed class StagingHeaderCurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid UserId => httpContextAccessor.HttpContext?.Items.TryGetValue(
        StagingTesterIdentityContext.UserIdItemKey,
        out var value) == true && value is Guid userId
        ? userId
        : throw new InvalidOperationException("Temporary tester identity is missing or invalid.");

    public bool IsAuthenticated => httpContextAccessor.HttpContext?.Items.ContainsKey(
        StagingTesterIdentityContext.UserIdItemKey) == true;
}
