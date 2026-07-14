using MacroViva.Application.Abstractions.Services;
using MacroViva.Api.Services;
using MacroViva.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace MacroViva.Api.Tests;

public sealed class CurrentUserServiceTests
{
    [Fact]
    public void DevelopmentCurrentUserService_UsesConfiguredDevelopmentUser()
    {
        var expectedUserId = Guid.Parse("11111111-1111-4111-8111-111111111111");
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["DevelopmentUser:UserId"] = expectedUserId.ToString()
            })
            .Build();

        var service = new DevelopmentCurrentUserService(configuration);

        Assert.True(service.IsAuthenticated);
        Assert.Equal(expectedUserId, service.UserId);
    }

    [Fact]
    public void StagingHeaderCurrentUserService_DoesNotUseDevelopmentFallback()
    {
        var httpContext = new DefaultHttpContext();
        var accessor = new HttpContextAccessor { HttpContext = httpContext };
        var service = new StagingHeaderCurrentUserService(accessor);

        Assert.False(service.IsAuthenticated);
        Assert.Throws<InvalidOperationException>(() => _ = service.UserId);
    }

    [Fact]
    public void StagingHeaderCurrentUserService_ReturnsResolvedUserId()
    {
        var expectedUserId = Guid.Parse("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa");
        var httpContext = new DefaultHttpContext();
        httpContext.Items[StagingTesterIdentityContext.UserIdItemKey] = expectedUserId;
        var service = new StagingHeaderCurrentUserService(new HttpContextAccessor { HttpContext = httpContext });

        Assert.True(service.IsAuthenticated);
        Assert.Equal(expectedUserId, service.UserId);
    }
}
