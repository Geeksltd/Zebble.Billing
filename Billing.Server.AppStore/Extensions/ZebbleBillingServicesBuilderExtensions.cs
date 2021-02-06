namespace Zebble.Billing
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class ZebbleBillingServicesBuilderExtensions
    {
        public static ZebbleBillingServicesBuilder AddAppStore(this ZebbleBillingServicesBuilder builder, string configKey = "ZebbleBilling:AppStore")
        {
            builder.Services.AddOptions<AppStoreOptions>()
                            .Configure<IConfiguration>((opts, config) => config.GetSection(configKey)?.Bind(opts))
                            .Validate(opts => opts.Validate());

            builder.Services.AddStoreConnector<AppStoreConnector>("AppStore");
            builder.Services.AddScoped<AppStoreHookInterceptor>();

            return builder;
        }
    }
}
