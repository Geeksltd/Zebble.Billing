namespace Zebble.Billing
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class ZebbleBillingServicesBuilderExtensions
    {
        public static ZebbleBillingServicesBuilder AddEntityFramework(this ZebbleBillingServicesBuilder builder)
        {
            builder.Services.AddOptions<DbContextOptions>()
                            .Configure(opts => builder.Configuration.GetSection("DbContext")?.Bind(opts))
                            .Validate(opts => opts.Validate());

            builder.Services.AddDbContext<BillingDbContext>();

            builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

            return builder;
        }
    }
}
