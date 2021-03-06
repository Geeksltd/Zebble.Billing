﻿namespace Zebble.Billing
{
    using Apple.Receipt.Verificator.Models;
    using Apple.Receipt.Verificator.Modules;
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
                            .Validate(opts => opts.HookInterceptorPath.HasValue(), $"{nameof(AppStoreOptions.HookInterceptorPath)} is empty.");

            builder.Services.AddOptions<AppleReceiptVerificationSettings>()
                            .Configure<IOptions<AppStoreOptions>>((settings, options) => options.Value.Apply(settings));
            builder.Services.RegisterAppleReceiptVerificator();

            builder.Services.AddStoreConnector<AppStoreConnector>("AppStore");
            builder.Services.AddScoped<AppStoreHookInterceptor>();

            return builder;
        }
    }
}
