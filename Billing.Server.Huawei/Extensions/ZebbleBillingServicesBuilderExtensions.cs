namespace Zebble.Billing
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Olive;

    public static class ZebbleBillingServicesBuilderExtensions
    {
        public static ZebbleBillingServicesBuilder AddHuawei(this ZebbleBillingServicesBuilder builder, string configKey = "ZebbleBilling:Huawei")
        {
            builder.Services.AddScoped<HuaweiDeveloperService>();

            builder.Services.AddOptions<HuaweiOptions>()
                            .Configure<IConfiguration>((opts, config) => config.GetSection(configKey)?.Bind(opts))
                            .Validate(opts => opts.PackageName.HasValue(), $"{nameof(HuaweiOptions.PackageName)} is empty.")
                            .Validate(opts => opts.PublicKey.HasValue(), $"{nameof(HuaweiOptions.PublicKey)} is empty.");

            builder.Services.AddStoreConnector<HuaweiConnector>("Huawei");

            return builder;
        }
    }
}
