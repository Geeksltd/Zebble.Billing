namespace Zebble.Billing
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Olive;

    public static partial class ZebbleBillingServicesBuilderExtensions
    {
        public static ZebbleBillingServicesBuilder AddEntityFramework(this ZebbleBillingServicesBuilder builder, string configKey = "ZebbleBilling:EntityFramework")
        {
            builder.Services.AddOptions<EntityFrameworkOptions>()
                            .Configure<IConfiguration>((opts, config) => config.GetSection(configKey)?.Bind(opts))
                            .Validate(opts => opts.ConnectionString.HasValue(), $"{nameof(EntityFrameworkOptions.ConnectionString)} is empty.");

            builder.Services.AddDbContext<SubscriptionDbContext>();

            builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

            return builder;
        }
    }
}
