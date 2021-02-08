namespace Zebble.Billing
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Olive;

    public static class ZebbleBillingAppBuilderExtensions
    {
        public static ZebbleBillingAppBuilder UseAppStore(this ZebbleBillingAppBuilder builder)
        {
            builder.App.MapWhen(MatchesHookInterceptorEndpoint, builder => builder.UseMiddleware<HookInterceptionMiddleware>());

            return builder;
        }

        static bool MatchesHookInterceptorEndpoint(HttpContext context)
        {
            var options = context.RequestServices.GetService<IOptionsSnapshot<AppStoreOptions>>();

            if (!context.Request.Path.StartsWithSegments(options.Value.HookInterceptorUri.AbsolutePath)) return false;

            if (!context.Request.Method.Equals("POST", caseSensitive: false)) return false;

            return true;
        }
    }
}
