namespace Zebble.Billing
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class ZebbleBillingServicesBuilderExtensions
    {
        public static ZebbleBillingServicesBuilder AddAppStore(this ZebbleBillingServicesBuilder builder)
        {
            builder.Services.Configure<AppStoreOptions>(opts => builder.Configuration.GetSection("AppStore")?.Bind(opts));
            builder.Services.AddScoped<AppStoreHookInterceptor>();

            return builder;
        }
    }
}
