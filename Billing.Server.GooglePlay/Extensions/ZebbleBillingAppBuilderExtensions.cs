namespace Zebble.Billing
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    public static class ZebbleBillingAppBuilderExtensions
    {
        public static ZebbleBillingAppBuilder UseGooglePlay(this ZebbleBillingAppBuilder builder)
        {
            var routes = new RouteBuilder(builder.App);

            var options = builder.App.ApplicationServices.GetService<IOptions<GooglePlayOptions>>();

            routes.MapMiddlewareGet(options.Value.QueueProcessorPath, builder => builder.UseMiddleware<GooglePlayQueueProcessingMiddleware>());

            builder.App.UseRouter(routes.Build());

            return builder;
        }
    }
}
