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
                            .Validate(opts => opts.PackageName.HasValue(), $"{nameof(GooglePlayOptions.PackageName)} is empty.")
                            .Validate(opts => opts.ProjectId.HasValue(), $"{nameof(GooglePlayOptions.ProjectId)} is empty.")
                            .Validate(opts => opts.PrivateKeyId.HasValue(), $"{nameof(GooglePlayOptions.PrivateKeyId)} is empty.")
                            .Validate(opts => opts.PrivateKey.HasValue(), $"{nameof(GooglePlayOptions.PrivateKey)} is empty.")
                            .Validate(opts => opts.ClientEmail.HasValue(), $"{nameof(GooglePlayOptions.ClientEmail)} is empty.")
                            .Validate(opts => opts.ClientId.HasValue(), $"{nameof(GooglePlayOptions.ClientId)} is empty.")
                            .Validate(opts => opts.SubscriptionId.HasValue(), $"{nameof(GooglePlayOptions.SubscriptionId)} is empty.")
                            .Validate(opts => opts.QueueProcessorPath.HasValue(), $"{nameof(GooglePlayOptions.QueueProcessorPath)} is empty.");

            builder.Services.AddStoreConnector<GooglePlayConnector>("GooglePlay");
            builder.Services.AddScoped<GooglePlayQueueProcessor>();

            return builder;
        }
    }
}
