namespace Zebble.Billing
{
    using CafeBazaar.DeveloperApi;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ZebbleBillingOptionsBuilderExtensions
    {
        public static ZebbleBillingOptionsBuilder AddCafeBazaar(this ZebbleBillingOptionsBuilder builder)
        {
            builder.Services.AddCafeBazaarDeveloperApi(builder.Configuration, "CafeBazaar:DeveloperApi");

            builder.Services.Configure<CafeBazaarOptions>(opts => builder.Configuration.GetSection("CafeBazaar")?.Bind(opts));
            builder.Services.AddScoped<ILiveSubscriptionQuery, CafeBazaarLiveSubscriptionQuery>();

            return builder;
        }
    }
}
