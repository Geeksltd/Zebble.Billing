namespace Zebble.Billing
{
    using Microsoft.AspNetCore.Builder;

    public static class ZebbleBillingAppBuilderExtensions
    {
        public static ZebbleBillingAppBuilder UseGooglePlay(this ZebbleBillingAppBuilder builder)
        {
            builder.App.UseMiddleware<GooglePlayQueueProcessingMiddleware>();

            return builder;
        }
    }
}
