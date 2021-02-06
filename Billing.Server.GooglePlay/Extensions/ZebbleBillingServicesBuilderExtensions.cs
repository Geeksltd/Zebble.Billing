namespace Zebble.Billing
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class ZebbleBillingServicesBuilderExtensions
    {
        public static ZebbleBillingServicesBuilder AddGooglePlay(this ZebbleBillingServicesBuilder builder, string configKey = "ZebbleBilling:GooglePlay")
        {
            builder.Services.AddOptions<GooglePlayOptions>()
                            .Configure<IConfiguration>((opts, config) => config.GetSection(configKey)?.Bind(opts))
                            .Validate(opts => opts.Validate());

            builder.Services.AddOptions<GooglePubSubOptions>()
                            .Configure<IConfiguration>((opts, config) => config.GetSection($"{configKey}:PubSub")?.Bind(opts))
                            .Validate(opts => opts.Validate());

            builder.Services.AddOptions<GooglePublisherOptions>()
                            .Configure<IConfiguration>((opts, config) => config.GetSection($"{configKey}:Publisher")?.Bind(opts))
                            .Validate(opts => opts.Validate());

            builder.Services.AddStoreConnector<GooglePlayConnector>("GooglePlay");
            builder.Services.AddScoped<GooglePlayQueueProcessor>();

            return builder;
        }
    }
}
