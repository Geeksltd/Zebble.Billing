namespace Zebble.Billing
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    public static class ZebbleBillingAppBuilderExtensions
    {
        public static ZebbleBillingAppBuilder UseHuawei(this ZebbleBillingAppBuilder builder)
        {
            var routes = new RouteBuilder(builder.App);

            var options = builder.App.ApplicationServices.GetService<IOptions<HuaweiOptions>>();

            routes.MapMiddlewarePost(options.Value.NotificationInterceptorPath, builder => builder.UseMiddleware<HuaweiNotificationInterceptionMiddleware>());

            builder.App.UseRouter(routes.Build());

            return builder;
        }
    }
}
