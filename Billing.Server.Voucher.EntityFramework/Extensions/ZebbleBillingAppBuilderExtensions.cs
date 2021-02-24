namespace Zebble.Billing
{
    using Microsoft.Extensions.DependencyInjection;

    public static class ZebbleBillingAppBuilderExtensions
    {
        public static ZebbleBillingVoucherAppBuilder UseEntityFramework(this ZebbleBillingVoucherAppBuilder builder)
        {
            builder.App.ApplicationServices.GetRequiredService<BillingDbContext>().Database.EnsureCreated();

            return builder;
        }
    }
}
