namespace Zebble.Billing
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public static class ZebbleBillingAppBuilderExtensions
    {
        public static ZebbleBillingVoucherAppBuilder UseEntityFramework(this ZebbleBillingVoucherAppBuilder builder)
        {
            using var serviceScope = builder.App.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            serviceScope.ServiceProvider.GetRequiredService<VoucherDbContext>().Database.Migrate();

            return builder;
        }
    }
}
