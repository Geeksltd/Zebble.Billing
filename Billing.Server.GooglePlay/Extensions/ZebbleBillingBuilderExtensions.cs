namespace Zebble.Billing
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class ZebbleBillingBuilderExtensions
    {
        public static IZebbleBillingBuilder AddGooglePlay(this IZebbleBillingBuilder builder)
        {
            builder.Services.Configure<GooglePlayOptions>(opts => builder.Configuration.GetSection("GooglePlay")?.Bind(opts));
            builder.Services.Configure<GooglePubSubOptions>(opts => builder.Configuration.GetSection("GooglePlay:PubSub")?.Bind(opts));
            builder.Services.Configure<GooglePublisherOptions>(opts => builder.Configuration.GetSection("GooglePlay:Publisher")?.Bind(opts));
            builder.Services.AddScoped<ILiveSubscriptionQuery, GooglePlayLiveSubscriptionQuery>();
            builder.Services.AddScoped<IQueueProcessor, GooglePlayQueueProcessor>();

            return builder;
        }
    }
}
