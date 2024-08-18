namespace Zebble.Billing
{
    using Apple.Receipt.Verificator.Models;
    using Apple.Receipt.Verificator.Modules;
    using AppStoreServerApi;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Olive;

    public static partial class ZebbleBillingServicesBuilderExtensions
    {
        public static ZebbleBillingServicesBuilder AddAppStore(this ZebbleBillingServicesBuilder builder, string configKey = "ZebbleBilling:AppStore")
        {
            builder.Services.AddOptions<AppStoreOptions>()
                            .Configure<IConfiguration>((opts, config) => config.GetSection(configKey)?.Bind(opts))
                            .Validate(opts => opts.PackageName.HasValue(), $"{nameof(AppStoreOptions.PackageName)} is empty.")
                            .Validate(opts => opts.SharedSecret.HasValue(), $"{nameof(AppStoreOptions.SharedSecret)} is empty.")
                            .Validate(opts => opts.PrivateKey.HasValue(), $"{nameof(AppStoreOptions.PrivateKey)} is empty.")
                            .Validate(opts => opts.KeyId.HasValue(), $"{nameof(AppStoreOptions.KeyId)} is empty.")
                            .Validate(opts => opts.IssuerId.HasValue(), $"{nameof(AppStoreOptions.IssuerId)} is empty.")
                            .Validate(opts => opts.HookInterceptorPath.HasValue(), $"{nameof(AppStoreOptions.HookInterceptorPath)} is empty.");

            // V1
            builder.Services.AddOptions<AppleReceiptVerificationSettings>()
                            .Configure<IOptions<AppStoreOptions>>((settings, options) => options.Value.Apply(settings));
            builder.Services.RegisterAppleReceiptVerificator();

            // V2
            builder.Services.AddScoped(sp =>
            {
                var options = sp.GetRequiredService<IOptions<AppStoreOptions>>().Value;

                return new AppStoreClient(
                    options.Environment == AppStoreEnvironment.Production ? AppleEnvironment.Production : AppleEnvironment.Sandbox,
                    options.PrivateKey,
                    options.KeyId,
                    options.IssuerId,
                    options.PackageName
                );
            });

            builder.Services.AddStoreConnector<AppStoreConnector>("AppStore");
            builder.Services.AddScoped<AppStoreHookInterceptor>();
            builder.Services.AddScoped<AppStoreHookInterceptorV2>();

            return builder;
        }
    }
}
