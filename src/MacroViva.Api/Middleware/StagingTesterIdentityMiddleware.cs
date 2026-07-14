using MacroViva.Api.Common;
using MacroViva.Application.Abstractions.Services;

namespace MacroViva.Api.Middleware;

public sealed class StagingTesterIdentityMiddleware(RequestDelegate next)
{
    public const string HeaderName = "X-MacroViva-Tester-Id";

    public async Task InvokeAsync(HttpContext context, IAnonymousBetaUserResolver betaUserResolver)
    {
        if (HttpMethods.IsOptions(context.Request.Method))
        {
            await next(context);
            return;
        }

        var endpointRequiresIdentity = context.GetEndpoint()?.Metadata.GetMetadata<RequiresTesterIdentityAttribute>() is not null;
        var hasHeader = context.Request.Headers.TryGetValue(HeaderName, out var headerValues);

        if (!hasHeader)
        {
            if (endpointRequiresIdentity)
            {
                await WriteInvalidIdentityResponseAsync(context);
                return;
            }

            await next(context);
            return;
        }

        if (headerValues.Count != 1 ||
            headerValues[0] is not { Length: 36 } rawInstallationId ||
            !Guid.TryParseExact(rawInstallationId, "D", out var installationId))
        {
            await WriteInvalidIdentityResponseAsync(context);
            return;
        }

        var userId = await betaUserResolver.ResolveOrCreateAsync(installationId, context.RequestAborted);
        context.Items[StagingTesterIdentityContext.UserIdItemKey] = userId;

        await next(context);
    }

    private static Task WriteInvalidIdentityResponseAsync(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        return context.Response.WriteAsJsonAsync(new
        {
            Code = "TesterIdentity.Invalid",
            Message = "Identificação temporária do tester ausente ou inválida."
        });
    }
}
