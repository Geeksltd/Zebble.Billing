namespace Zebble.Billing
{
    using CafeBazaar.DeveloperApi;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ZebbleBillingServicesBuilderExtensions
    {
        public static ZebbleBillingServicesBuilder AddCafeBazaar(this ZebbleBillingServicesBuilder builder)
        {
            var cafeBazaarConfig = builder.Configuration.GetSection("CafeBazaar");

            builder.Services.AddCafeBazaarDeveloperApi(cafeBazaarConfig, "CafeBazaar:DeveloperApi");

            builder.Services.AddOptions<CafeBazaarOptions>()
                            .Configure(opts => cafeBazaarConfig?.Bind(opts))
                            .Validate(opts => opts.Validate());

            builder.Services.AddStoreConnector<CafeBazaarConnector>("CafeBazaar");

            return builder;
        }
    }
}
