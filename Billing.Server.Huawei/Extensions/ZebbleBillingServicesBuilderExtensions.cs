namespace Zebble.Billing
{
    using Huawei.DeveloperApi;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Olive;

    public static class ZebbleBillingServicesBuilderExtensions
    {
        public static ZebbleBillingServicesBuilder AddHuawei(this ZebbleBillingServicesBuilder builder, string configKey = "ZebbleBilling:Huawei")
        {
            builder.Services.AddHuaweiDeveloperApi(ConfigurationPath.Combine(configKey, "DeveloperApi"));

            builder.Services.AddOptions<HuaweiOptions>()
                            .Configure<IConfiguration>((opts, config) => config.GetSection(configKey)?.Bind(opts))
                            .Validate(opts => opts.PublicKey.HasValue(), $"{nameof(HuaweiOptions.PublicKey)} is empty.");

            builder.Services.AddStoreConnector<HuaweiConnector>("Huawei");
            builder.Services.AddScoped<HuaweiNotificationInterceptor>();

            return builder;
        }
    }
}
