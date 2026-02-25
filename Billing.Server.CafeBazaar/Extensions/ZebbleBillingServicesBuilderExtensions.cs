namespace Zebble.Billing
{
    using CafeBazaar.DeveloperApi;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Olive;

    public static class ZebbleBillingServicesBuilderExtensions
    {
        public static ZebbleBillingServicesBuilder AddCafeBazaar(this ZebbleBillingServicesBuilder builder, string configKey = "ZebbleBilling:CafeBazaar")
        {
            builder.Services.AddCafeBazaarDeveloperApi($"{configKey}:DeveloperApi");

            builder.Services.AddOptions<CafeBazaarOptions>()
                            .Configure<IConfiguration>((opts, config) =>
                            {
                                config.GetSection(configKey)?.Bind(opts);
                                config.GetSection(ConfigurationPath.KeyDelimiter + configKey)?.Bind(opts);
                            })
                            .Validate(opts => opts.PackageName.HasValue(), $"{nameof(CafeBazaarOptions.PackageName)} is empty.");

            builder.Services.AddStoreConnector<CafeBazaarConnector>("CafeBazaar");

            return builder;
        }
    }
}
