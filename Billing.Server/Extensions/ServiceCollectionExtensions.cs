namespace Zebble.Billing
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddZebbleBilling(this IServiceCollection services, IConfiguration config, Action<ZebbleBillingServicesBuilder> builder = null)
        {
            var zebbleBillingConfig = config.GetSection("ZebbleBilling");

            services.AddZebbleProductsCache(zebbleBillingConfig);

            services.AddScoped<SubscriptionManager>();

            builder?.Invoke(new ZebbleBillingServicesBuilder(services, zebbleBillingConfig));

            return services;
        }
    }
}
