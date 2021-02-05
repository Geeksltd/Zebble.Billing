namespace Zebble.Billing
{
    using Microsoft.AspNetCore.Builder;

    public static class ZebbleBillingConfiguratorExtensions
    {
        public static IZebbleBillingConfigurator UseGooglePlay(this IZebbleBillingConfigurator configurator)
        {
            configurator.App.UseMiddleware<GooglePlayQueueProcessingMiddleware>();

            return configurator;
        }
    }
}
