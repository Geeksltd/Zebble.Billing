namespace Zebble.Billing
{
    using Apple.Receipt.Verificator.Modules;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Olive;

    public static partial class ZebbleBillingServicesBuilderExtensions
    {
        public static ZebbleBillingServicesBuilder AddAppStore(this ZebbleBillingServicesBuilder builder, string configKey = "ZebbleBilling:AppStore")
        {
            builder.Services.AddOptions<AppStoreOptions>()
                            .Configure<IConfiguration>((opts, config) => config.GetSection(configKey)?.Bind(opts))
                            .Validate(opts => opts.PackageName.IsEmpty(), $"{nameof(AppStoreOptions.PackageName)} is empty.")
                            .Validate(opts => opts.SharedSecret.IsEmpty(), $"{nameof(AppStoreOptions.SharedSecret)} is empty.")
                            .Validate(opts => opts.HookInterceptorUri == null, $"{nameof(AppStoreOptions.HookInterceptorUri)} is null.")
                            .Validate(opts => opts.HookInterceptorUri.IsAbsoluteUri == false, $"{nameof(AppStoreOptions.HookInterceptorUri)} should be absolute.")
                            .PostConfigure(opts => builder.Services.RegisterAppleReceiptVerificator(opts.Apply));

            builder.Services.AddStoreConnector<AppStoreConnector>("AppStore");
            builder.Services.AddScoped<AppStoreHookInterceptor>();

            return builder;
        }
    }
}
