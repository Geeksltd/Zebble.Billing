namespace Zebble.Billing
{
    using Apple.Receipt.Verificator.Models;
    using Apple.Receipt.Verificator.Modules;
    using Microsoft.AspNetCore.Http;
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
                            .Validate(opts => opts.HookInterceptorUri is not null, $"{nameof(AppStoreOptions.HookInterceptorUri)} is null.")
                            .PostConfigure<IHttpContextAccessor>((opts, contextAccessor) =>
                            {
                                if (opts.HookInterceptorUri.IsAbsoluteUri) return;
                                opts.HookInterceptorUri = contextAccessor.ToAbsolute(opts.HookInterceptorUri);
                            });

            builder.Services.AddOptions<AppleReceiptVerificationSettings>()
                            .Configure<IOptions<AppStoreOptions>>((settings, options) => options.Value.Apply(settings));
            builder.Services.RegisterAppleReceiptVerificator();

            builder.Services.AddStoreConnector<AppStoreConnector>("AppStore");
            builder.Services.AddScoped<AppStoreHookInterceptor>();

            return builder;
        }
    }
}
