namespace Zebble.Billing;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public static class ZebbleBillingAppBuilderExtensions
{
    public static ZebbleBillingAppBuilder UseAppStoreV2(this ZebbleBillingAppBuilder builder)
    {
        var routes = new RouteBuilder(builder.App);

        var options = builder.App.ApplicationServices.GetService<IOptions<AppStoreOptions>>();

        routes.MapMiddlewarePost(options.Value.HookInterceptorPath, builder => builder.UseMiddleware<AppStoreHookInterceptionMiddleware>());

        builder.App.UseRouter(routes.Build());

        return builder;
    }
}
