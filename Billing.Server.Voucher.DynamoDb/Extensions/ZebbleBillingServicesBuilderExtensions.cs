namespace Zebble.Billing
{
    using Microsoft.Extensions.DependencyInjection;

    public static partial class ZebbleBillingServicesBuilderExtensions
    {
        public static ZebbleBillingVoucherServicesBuilder AddDynamoDb(this ZebbleBillingVoucherServicesBuilder builder)
        {
            builder.Services.AddScoped<VoucherDbContext>();

            builder.Services.AddScoped<IVoucherRepository, VoucherRepository>();

            return builder;
        }
    }
}
