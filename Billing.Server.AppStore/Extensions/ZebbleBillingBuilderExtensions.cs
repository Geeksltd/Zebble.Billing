namespace Zebble.Billing
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class ZebbleBillingBuilderExtensions
    {
        public static IZebbleBillingBuilder AddAppStore(this IZebbleBillingBuilder builder)
        {
            builder.Services.Configure<AppStoreOptions>(opts => builder.Configuration.GetSection("AppStore")?.Bind(opts));
            builder.Services.AddScoped<IHookInterceptor, AppStoreHookInterceptor>();

            return builder;
        }
    }
}
