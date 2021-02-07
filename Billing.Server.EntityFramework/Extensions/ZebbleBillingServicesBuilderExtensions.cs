namespace Zebble.Billing
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Olive;

    public static partial class ZebbleBillingServicesBuilderExtensions
    {
        public static ZebbleBillingServicesBuilder AddEntityFramework(this ZebbleBillingServicesBuilder builder, string configKey = "ZebbleBilling:DbContext")
        {
            builder.Services.AddOptions<DbContextOptions>()
                            .Configure<IConfiguration>((opts, config) => config.GetSection(configKey)?.Bind(opts))
                            .Validate(opts => opts.ConnectionString.IsEmpty(), $"{nameof(DbContextOptions.ConnectionString)} is empty.");

            builder.Services.AddDbContext<BillingDbContext>();

            builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

            return builder;
        }
    }
}
