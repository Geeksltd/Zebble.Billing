namespace Zebble.Billing
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class ZebbleBillingServicesBuilderExtensions
    {
        public static ZebbleBillingServicesBuilder AddGooglePlay(this ZebbleBillingServicesBuilder builder)
        {
            builder.Services.AddOptions<GooglePlayOptions>()
                            .Configure(opts => builder.Configuration.GetSection("GooglePlay")?.Bind(opts))
                            .Validate(opts => opts.Validate());

            builder.Services.AddOptions<GooglePubSubOptions>()
                            .Configure(opts => builder.Configuration.GetSection("GooglePlay:PubSub")?.Bind(opts))
                            .Validate(opts => opts.Validate());

            builder.Services.AddOptions<GooglePublisherOptions>()
                            .Configure(opts => builder.Configuration.GetSection("GooglePlay:Publisher")?.Bind(opts))
                            .Validate(opts => opts.Validate());

            builder.Services.AddStoreConnector<GooglePlayConnector>("GooglePlay");
            builder.Services.AddScoped<GooglePlayQueueProcessor>();

            return builder;
        }
    }
}
