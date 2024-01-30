namespace Zebble.Billing
{
    using Google.Apis.AndroidPublisher.v3;
    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Services;
    using Google.Cloud.PubSub.V1;
    using Grpc.Auth;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Olive;
    using System.Text.Json;

    public static partial class ZebbleBillingServicesBuilderExtensions
    {
        public static ZebbleBillingServicesBuilder AddGooglePlay(this ZebbleBillingServicesBuilder builder, string configKey = "ZebbleBilling:GooglePlay")
        {
            builder.Services.AddOptions<GooglePlayOptions>()
                            .Configure<IConfiguration>((opts, config) => config.GetSection(configKey)?.Bind(opts))
                            .Validate(opts => opts.PackageName.HasValue(), $"{nameof(GooglePlayOptions.PackageName)} is empty.")
                            .Validate(opts => opts.Store.ProjectId.HasValue(), $"{nameof(GooglePlayStoreOptions.ProjectId)} is empty.")
                            .Validate(opts => opts.Store.PrivateKeyId.HasValue(), $"{nameof(GooglePlayStoreOptions.PrivateKeyId)} is empty.")
                            .Validate(opts => opts.Store.PrivateKey.HasValue(), $"{nameof(GooglePlayStoreOptions.PrivateKey)} is empty.")
                            .Validate(opts => opts.Store.ClientEmail.HasValue(), $"{nameof(GooglePlayStoreOptions.ClientEmail)} is empty.")
                            .Validate(opts => opts.Store.ClientId.HasValue(), $"{nameof(GooglePlayStoreOptions.ClientId)} is empty.")
                            .Validate(opts => opts.PubSub.ProjectId.HasValue(), $"{nameof(GooglePlayStoreOptions.ProjectId)} is empty.")
                            .Validate(opts => opts.PubSub.PrivateKeyId.HasValue(), $"{nameof(GooglePlayPubSubOptions.PrivateKeyId)} is empty.")
                            .Validate(opts => opts.PubSub.PrivateKey.HasValue(), $"{nameof(GooglePlayPubSubOptions.PrivateKey)} is empty.")
                            .Validate(opts => opts.PubSub.ClientEmail.HasValue(), $"{nameof(GooglePlayPubSubOptions.ClientEmail)} is empty.")
                            .Validate(opts => opts.PubSub.ClientId.HasValue(), $"{nameof(GooglePlayPubSubOptions.ClientId)} is empty.")
                            .Validate(opts => opts.PubSub.SubscriptionId.HasValue(), $"{nameof(GooglePlayPubSubOptions.SubscriptionId)} is empty.")
                            .Validate(opts => opts.QueueProcessorPath.HasValue(), $"{nameof(GooglePlayOptions.QueueProcessorPath)} is empty.");

            builder.Services.AddSingleton(sp =>
            {
                var options = sp.GetService<IOptions<GooglePlayOptions>>().Value.Store;
                var initializer = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(options.ClientEmail)
                {
                    ProjectId = options.ProjectId,
                    KeyId = options.PrivateKeyId,
                    Scopes = new[] { AndroidPublisherService.ScopeConstants.Androidpublisher }
                }.FromPrivateKey(options.PrivateKey));

                return new AndroidPublisherService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = initializer
                });
            });

            builder.Services.AddTransient(sp =>
            {
                var options = sp.GetService<IOptions<GooglePlayOptions>>().Value.PubSub;

                var json = new JsonCredentialParameters
                {
                    Type = JsonCredentialParameters.ServiceAccountCredentialType,
                    ProjectId = options.ProjectId,
                    PrivateKeyId = options.PrivateKeyId,
                    PrivateKey = options.PrivateKey,
                    ClientEmail = options.ClientEmail,
                    ClientId = options.ClientId
                }.ToJson(new JsonSerializerOptions { PropertyNamingPolicy = new SnakeCasePropertyNamingPolicy() });

                return new SubscriberClientBuilder
                {
                    ChannelCredentials = GoogleCredential.FromJson(json).ToChannelCredentials(),
                    SubscriptionName = new SubscriptionName(options.ProjectId, options.SubscriptionId)
                }.Build(sp);
            });

            builder.Services.AddStoreConnector<GooglePlayConnector>("GooglePlay");
            builder.Services.AddScoped<GooglePlayQueueProcessor>();

            return builder;
        }
    }

    class SnakeCasePropertyNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) => name.ToSnakeCase();
    }
}
