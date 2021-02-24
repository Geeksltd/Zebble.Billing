namespace Zebble.Billing
{
    using Microsoft.Extensions.DependencyInjection;

    public static partial class ZebbleBillingServicesBuilderExtensions
    {
        public static ZebbleBillingVoucherServicesBuilder AddEntityFramework(this ZebbleBillingVoucherServicesBuilder builder)
        {
            builder.Services.AddDbContext<VoucherDbContext>();

            builder.Services.AddScoped<IVoucherRepository, VoucherRepository>();

            return builder;
        }
    }
}
