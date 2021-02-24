namespace Zebble.Billing
{
    using Microsoft.Extensions.DependencyInjection;

    public static class ZebbleBillingAppBuilderExtensions
    {
        public static ZebbleBillingAppBuilder UseEntityFramework(this ZebbleBillingAppBuilder builder)
        {
            builder.App.ApplicationServices.GetRequiredService<BillingDbContext>().Database.EnsureCreated();

            return builder;
        }
    }
}
