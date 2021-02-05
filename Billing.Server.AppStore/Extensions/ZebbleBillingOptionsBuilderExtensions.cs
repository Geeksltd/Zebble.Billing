namespace Zebble.Billing
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class ZebbleBillingOptionsBuilderExtensions
    {
        public static ZebbleBillingOptionsBuilder AddAppStore(this ZebbleBillingOptionsBuilder builder)
        {
            builder.Services.Configure<AppStoreOptions>(opts => builder.Configuration.GetSection("AppStore")?.Bind(opts));
            builder.Services.AddScoped<IHookInterceptor, AppStoreHookInterceptor>();

            return builder;
        }
    }
}
