namespace Zebble.Billing
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseZebbleBilling(this IApplicationBuilder app, Action<ZebbleBillingAppBuilder> configurator = null)
        {
            configurator?.Invoke(new ZebbleBillingAppBuilder(app));

            var options = app.ApplicationServices.GetService<IOptions<BillingOptions>>();

            var routes = new RouteBuilder(app);

            routes.MapMiddlewarePost(options.Value.VerifyPurchasePath, builder => builder.UseMiddleware<AppVerifyPurchaseMiddleware>());
            routes.MapMiddlewarePost(options.Value.PurchaseAttemptPath, builder => builder.UseMiddleware<AppPurchaseAttemptMiddleware>());
            routes.MapMiddlewarePost(options.Value.SubscriptionStatusPath, builder => builder.UseMiddleware<AppSubscriptionStatusMiddleware>());

            app.UseRouter(routes.Build());

            return app;
        }
    }
}
