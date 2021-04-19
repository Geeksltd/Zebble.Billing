namespace Zebble.Billing
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Olive;

    public static partial class ZebbleBillingServicesBuilderExtensions
    {
        public static ZebbleBillingServicesBuilder AddDynamoDb(this ZebbleBillingServicesBuilder builder, string configKey = "ZebbleBilling:DynamoDb")
        {
            builder.Services.AddOptions<DynamoDbOptions>()
                            .Configure<IConfiguration>((opts, config) => config.GetSection(configKey)?.Bind(opts))
                            .Validate(opts => opts.AccessKey.HasValue(), $"{nameof(DynamoDbOptions.AccessKey)} is empty.")
                            .Validate(opts => opts.SecretKey.HasValue(), $"{nameof(DynamoDbOptions.SecretKey)} is empty.");

            builder.Services.AddScoped(sp => sp.GetRequiredService<IOptions<DynamoDbOptions>>().Value.CreateClient());

            builder.Services.AddScoped<SubscriptionDbContext>();

            builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

            return builder;
        }
    }
}
