namespace Zebble.Billing
{
    using CafeBazaar.DeveloperApi;

    public static class ZebbleBillingConfiguratorExtensions
    {
        public static IZebbleBillingConfigurator UseCafeBazaar(this IZebbleBillingConfigurator configurator)
        {
            configurator.App.UseCafeBazaarDeveloperApi();

            return configurator;
        }
    }
}
