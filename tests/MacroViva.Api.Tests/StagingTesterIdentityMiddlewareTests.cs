using MacroViva.Api.Common;
using MacroViva.Api.Middleware;
using MacroViva.Application.Abstractions.Services;
using Microsoft.AspNetCore.Http;

namespace MacroViva.Api.Tests;

public sealed class StagingTesterIdentityMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ReturnsBadRequestForInvalidHeader()
    {
        var context = CreateContext(requiresIdentity: true);
        context.Request.Headers[StagingTesterIdentityMiddleware.HeaderName] = "invalid";
        var middleware = new StagingTesterIdentityMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context, new FakeResolver());

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_ReturnsBadRequestWhenRequiredHeaderIsMissing()
    {
        var context = CreateContext(requiresIdentity: true);
        var middleware = new StagingTesterIdentityMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context, new FakeResolver());

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_AllowsPublicEndpointWithoutHeader()
    {
        var nextCalled = false;
        var context = CreateContext(requiresIdentity: false);
        var middleware = new StagingTesterIdentityMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        await middleware.InvokeAsync(context, new FakeResolver());

        Assert.True(nextCalled);
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_StoresOnlyResolvedInternalUserId()
    {
        var context = CreateContext(requiresIdentity: true);
        context.Request.Headers[StagingTesterIdentityMiddleware.HeaderName] = "11111111-1111-4111-8111-111111111111";
        var expectedUserId = Guid.Parse("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa");
        var middleware = new StagingTesterIdentityMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context, new FakeResolver(expectedUserId));

        Assert.Equal(expectedUserId, context.Items[StagingTesterIdentityContext.UserIdItemKey]);
        Assert.DoesNotContain(context.Items.Values, value => value is string);
    }

    private static DefaultHttpContext CreateContext(bool requiresIdentity)
    {
        var context = new DefaultHttpContext();

        if (requiresIdentity)
        {
            context.SetEndpoint(new Endpoint(
                _ => Task.CompletedTask,
                new EndpointMetadataCollection(new RequiresTesterIdentityAttribute()),
                "stateful"));
        }

        return context;
    }

    private sealed class FakeResolver(Guid? userId = null) : IAnonymousBetaUserResolver
    {
        public Task<Guid> ResolveOrCreateAsync(Guid installationId, CancellationToken cancellationToken)
        {
            return Task.FromResult(userId ?? Guid.Parse("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa"));
        }
    }
}
