namespace Zebble.Billing
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    public static class ZebbleBillingAppBuilderExtensions
    {
        public static ZebbleBillingAppBuilder UseAppStore(this ZebbleBillingAppBuilder builder)
        {
            builder.App.MapWhen(MatchesHookInterceptorEndpoint, builder => builder.UseMiddleware<AppStoreHookInterceptionMiddleware>());

            return builder;
        }

        static bool MatchesHookInterceptorEndpoint(HttpContext context)
        {
            var options = context.RequestServices.GetService<IOptionsSnapshot<AppStoreOptions>>();

            if (!context.Request.Matches(options.Value.HookInterceptorUri)) return false;

            if (!context.Request.IsPost()) return false;

            return true;
        }
    }
}
