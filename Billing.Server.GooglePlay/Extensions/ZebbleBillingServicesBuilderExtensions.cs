namespace Zebble.Billing
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class ZebbleBillingServicesBuilderExtensions
    {
        public static ZebbleBillingServicesBuilder AddGooglePlay(this ZebbleBillingServicesBuilder builder)
        {
            builder.Services.Configure<GooglePlayOptions>(opts => builder.Configuration.GetSection("GooglePlay")?.Bind(opts));
            builder.Services.Configure<GooglePubSubOptions>(opts => builder.Configuration.GetSection("GooglePlay:PubSub")?.Bind(opts));
            builder.Services.Configure<GooglePublisherOptions>(opts => builder.Configuration.GetSection("GooglePlay:Publisher")?.Bind(opts));
            builder.Services.AddScoped<IPlatformProvider, GooglePlayPlatformProvider>();
            builder.Services.AddScoped<GooglePlayQueueProcessor>();

            return builder;
        }
    }
}
