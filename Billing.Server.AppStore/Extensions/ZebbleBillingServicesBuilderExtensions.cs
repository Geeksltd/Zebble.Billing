namespace Zebble.Billing
{
    using Apple.Receipt.Verificator.Modules;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class ZebbleBillingServicesBuilderExtensions
    {
        public static ZebbleBillingServicesBuilder AddAppStore(this ZebbleBillingServicesBuilder builder, string configKey = "ZebbleBilling:AppStore")
        {
            builder.Services.AddOptions<AppStoreOptions>()
                            .Configure<IConfiguration>((opts, config) => config.GetSection(configKey)?.Bind(opts))
                            .Validate(opts => opts.Validate())
                            .PostConfigure(opts => builder.Services.RegisterAppleReceiptVerificator(opts.Apply));

            builder.Services.AddStoreConnector<AppStoreConnector>("AppStore");
            builder.Services.AddScoped<AppStoreHookInterceptor>();

            return builder;
        }
    }
}
