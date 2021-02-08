namespace Zebble.Billing
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Olive;

    public static class ZebbleBillingAppBuilderExtensions
    {
        public static ZebbleBillingAppBuilder UseGooglePlay(this ZebbleBillingAppBuilder builder)
        {
            builder.App.MapWhen(MatchesQueueProcessorEndpoint, builder => builder.UseMiddleware<GooglePlayQueueProcessingMiddleware>());

            return builder;
        }

        static bool MatchesQueueProcessorEndpoint(HttpContext context)
        {
            var options = context.RequestServices.GetService<IOptionsSnapshot<GooglePlayOptions>>();

            if (!context.Request.Path.StartsWithSegments(options.Value.QueueProcessorUri.AbsolutePath)) return false;

            if (!context.Request.Method.Equals("POST", caseSensitive: false)) return false;

            return true;
        }
    }
}
