namespace Zebble.Billing
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Olive;

    public static partial class ZebbleBillingServicesBuilderExtensions
    {
        public static ZebbleBillingServicesBuilder AddGooglePlay(this ZebbleBillingServicesBuilder builder, string configKey = "ZebbleBilling:GooglePlay")
        {
            builder.Services.AddOptions<GooglePlayOptions>()
                            .Configure<IConfiguration>((opts, config) => config.GetSection(configKey)?.Bind(opts))
                            .Validate(opts => opts.PackageName.IsEmpty(), $"{nameof(GooglePlayOptions.PackageName)} is empty.")
                            .Validate(opts => opts.QueueProcessorUri == null, $"{nameof(GooglePlayOptions.QueueProcessorUri)} is null.")
                            .Validate(opts => opts.QueueProcessorUri.IsAbsoluteUri == false, $"{nameof(GooglePlayOptions.QueueProcessorUri)} should be absolute.");

            builder.Services.AddOptions<GooglePubSubOptions>()
                            .Configure<IConfiguration>((opts, config) => config.GetSection($"{configKey}:PubSub")?.Bind(opts))
                            .Validate(opts => opts.ProjectId.IsEmpty(), $"{nameof(GooglePubSubOptions.ProjectId)} is empty.")
                            .Validate(opts => opts.PrivateKeyId.IsEmpty(), $"{nameof(GooglePubSubOptions.PrivateKeyId)} is empty.")
                            .Validate(opts => opts.PrivateKey.IsEmpty(), $"{nameof(GooglePubSubOptions.PrivateKey)} is empty.")
                            .Validate(opts => opts.ClientEmail.IsEmpty(), $"{nameof(GooglePubSubOptions.ClientEmail)} is empty.")
                            .Validate(opts => opts.ClientId.IsEmpty(), $"{nameof(GooglePubSubOptions.ClientId)} is empty.")
                            .Validate(opts => opts.SubscriptionId.IsEmpty(), $"{nameof(GooglePubSubOptions.SubscriptionId)} is empty.");

            builder.Services.AddOptions<GooglePublisherOptions>()
                            .Configure<IConfiguration>((opts, config) => config.GetSection($"{configKey}:Publisher")?.Bind(opts))
                            .Validate(opts => opts.ProjectId.IsEmpty(), $"{nameof(GooglePublisherOptions.ProjectId)} is empty.")
                            .Validate(opts => opts.PrivateKeyId.IsEmpty(), $"{nameof(GooglePublisherOptions.PrivateKeyId)} is empty.")
                            .Validate(opts => opts.PrivateKey.IsEmpty(), $"{nameof(GooglePublisherOptions.PrivateKey)} is empty.")
                            .Validate(opts => opts.ClientEmail.IsEmpty(), $"{nameof(GooglePublisherOptions.ClientEmail)} is empty.")
                            .Validate(opts => opts.ClientId.IsEmpty(), $"{nameof(GooglePublisherOptions.ClientId)} is empty.");

            builder.Services.AddStoreConnector<GooglePlayConnector>("GooglePlay");
            builder.Services.AddScoped<GooglePlayQueueProcessor>();

            return builder;
        }
    }
}
