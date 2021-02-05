namespace Zebble.Billing
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddZebbleBilling(this IServiceCollection services, IConfiguration config, Action<IZebbleBillingBuilder> builder = null)
        {
            var zebbleBillingConfig = config.GetSection("ZebbleBilling");

            services.AddZebbleProductsCache(zebbleBillingConfig);

            services.AddScoped(typeof(IPlatformProvider<>), typeof(PlatformProvider<>));

            services.AddScoped<ISubscriptionManager, SubscriptionManager>();

            builder?.Invoke(new ZebbleBillingBuilder(services, zebbleBillingConfig));

            return services;
        }
    }
}
