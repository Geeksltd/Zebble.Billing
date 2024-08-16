namespace Zebble.Billing;

using AppStoreServerApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Olive;

public static partial class ZebbleBillingServicesBuilderExtensions
{
    public static ZebbleBillingServicesBuilder AddAppStoreV2(this ZebbleBillingServicesBuilder builder, string configKey = "ZebbleBilling:AppStoreV2")
    {
        builder.Services.AddOptions<AppStoreOptions>()
                        .Configure<IConfiguration>((opts, config) => config.GetSection(configKey)?.Bind(opts))
                        .Validate(opts => opts.PackageName.HasValue(), $"{nameof(AppStoreOptions.PackageName)} is empty.")
                        .Validate(opts => opts.PrivateKey.HasValue(), $"{nameof(AppStoreOptions.PrivateKey)} is empty.")
                        .Validate(opts => opts.KeyId.HasValue(), $"{nameof(AppStoreOptions.KeyId)} is empty.")
                        .Validate(opts => opts.IssuerId.HasValue(), $"{nameof(AppStoreOptions.IssuerId)} is empty.")
                        .Validate(opts => opts.HookInterceptorPath.HasValue(), $"{nameof(AppStoreOptions.HookInterceptorPath)} is empty.");

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

        return builder;
    }
}
