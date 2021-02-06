namespace Zebble.Billing
{
    using CafeBazaar.DeveloperApi;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ZebbleBillingServicesBuilderExtensions
    {
        public static ZebbleBillingServicesBuilder AddCafeBazaar(this ZebbleBillingServicesBuilder builder, string configKey = "ZebbleBilling:CafeBazaar")
        {
            builder.Services.AddCafeBazaarDeveloperApi($"{configKey}:DeveloperApi");

            builder.Services.AddOptions<CafeBazaarOptions>()
                            .Configure<IConfiguration>((opts, config) => config.GetSection(configKey)?.Bind(opts))
                            .Validate(opts => opts.Validate());

            builder.Services.AddStoreConnector<CafeBazaarConnector>("CafeBazaar");

            return builder;
        }
    }
}
