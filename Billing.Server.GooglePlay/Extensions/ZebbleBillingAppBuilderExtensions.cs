namespace Zebble.Billing
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

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

            if (!context.Request.Matches(options.Value.QueueProcessorUri)) return false;

            if (!context.Request.IsPost()) return false;

            return true;
        }
    }
}
