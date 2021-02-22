namespace Zebble.Billing
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Olive;

    public static partial class ZebbleBillingServicesBuilderExtensions
    {
        public static ZebbleBillingServicesBuilder AddGooglePlay(this ZebbleBillingServicesBuilder builder, string configKey = "ZebbleBilling:GooglePlay")
        {
            builder.Services.AddOptions<GooglePlayOptions>()
                            .Configure<IConfiguration>((opts, config) => config.GetSection(configKey)?.Bind(opts))
                            .PostConfigure<IHttpContextAccessor>((opts, contextAccessor) =>
                            {
                                if (opts.QueueProcessorUri.IsAbsoluteUri) return;
                                opts.QueueProcessorUri = contextAccessor.ToAbsolute(opts.QueueProcessorUri);
                            })
                            .Validate(opts => opts.PackageName.HasValue(), $"{nameof(GooglePlayOptions.PackageName)} is empty.")
                            .Validate(opts => opts.QueueProcessorUri is not null, $"{nameof(GooglePlayOptions.QueueProcessorUri)} is null.");

            builder.Services.AddOptions<GooglePubSubOptions>()
                            .Configure<IConfiguration>((opts, config) => config.GetSection($"{configKey}:PubSub")?.Bind(opts))
                            .Validate(opts => opts.ProjectId.HasValue(), $"{nameof(GooglePubSubOptions.ProjectId)} is empty.")
                            .Validate(opts => opts.PrivateKeyId.HasValue(), $"{nameof(GooglePubSubOptions.PrivateKeyId)} is empty.")
                            .Validate(opts => opts.PrivateKey.HasValue(), $"{nameof(GooglePubSubOptions.PrivateKey)} is empty.")
                            .Validate(opts => opts.ClientEmail.HasValue(), $"{nameof(GooglePubSubOptions.ClientEmail)} is empty.")
                            .Validate(opts => opts.ClientId.HasValue(), $"{nameof(GooglePubSubOptions.ClientId)} is empty.")
                            .Validate(opts => opts.SubscriptionId.HasValue(), $"{nameof(GooglePubSubOptions.SubscriptionId)} is empty.");

            builder.Services.AddOptions<GooglePublisherOptions>()
                            .Configure<IConfiguration>((opts, config) => config.GetSection($"{configKey}:Publisher")?.Bind(opts))
                            .Validate(opts => opts.ProjectId.HasValue(), $"{nameof(GooglePublisherOptions.ProjectId)} is empty.")
                            .Validate(opts => opts.PrivateKeyId.HasValue(), $"{nameof(GooglePublisherOptions.PrivateKeyId)} is empty.")
                            .Validate(opts => opts.PrivateKey.HasValue(), $"{nameof(GooglePublisherOptions.PrivateKey)} is empty.")
                            .Validate(opts => opts.ClientEmail.HasValue(), $"{nameof(GooglePublisherOptions.ClientEmail)} is empty.")
                            .Validate(opts => opts.ClientId.HasValue(), $"{nameof(GooglePublisherOptions.ClientId)} is empty.");

            builder.Services.AddStoreConnector<GooglePlayConnector>("GooglePlay");
            builder.Services.AddScoped<GooglePlayQueueProcessor>();

            return builder;
        }
    }
}
