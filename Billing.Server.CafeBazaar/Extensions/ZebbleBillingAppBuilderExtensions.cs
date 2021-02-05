namespace Zebble.Billing
{
    using CafeBazaar.DeveloperApi;

    public static class ZebbleBillingAppBuilderExtensions
    {
        public static ZebbleBillingAppBuilder UseCafeBazaar(this ZebbleBillingAppBuilder builder)
        {
            builder.App.UseCafeBazaarDeveloperApi();

            return builder;
        }
    }
}
