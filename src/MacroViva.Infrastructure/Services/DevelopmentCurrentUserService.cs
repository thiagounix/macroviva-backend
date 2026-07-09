using MacroViva.Application.Abstractions.Services;
using Microsoft.Extensions.Configuration;

namespace MacroViva.Infrastructure.Services;

public sealed class DevelopmentCurrentUserService(IConfiguration configuration) : ICurrentUserService
{
    private static readonly Guid DefaultDevelopmentUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public Guid UserId
    {
        get
        {
            var configuredValue = configuration["DevelopmentUser:UserId"];

            return Guid.TryParse(configuredValue, out var userId) ? userId : DefaultDevelopmentUserId;
        }
    }

    public bool IsAuthenticated => true;
}
