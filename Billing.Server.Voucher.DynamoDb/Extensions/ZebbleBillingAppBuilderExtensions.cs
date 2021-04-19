namespace Zebble.Billing
{
    public static class ZebbleBillingAppBuilderExtensions
    {
        public static ZebbleBillingVoucherAppBuilder UseDynamoDb(this ZebbleBillingVoucherAppBuilder builder) => builder;
    }
}
