namespace Zebble.Billing
{
    using System;
    using System.Linq;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Olive;

    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddZebbleBilling(this IServiceCollection services, IConfiguration config, Action<ZebbleBillingServicesBuilder> builder = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            if (config == null) throw new ArgumentNullException(nameof(config));

            var zebbleBillingConfig = config.GetSection("ZebbleBilling");

            services.AddZebbleProductsCache(zebbleBillingConfig);

            services.AddScoped<SubscriptionManager>();

            builder?.Invoke(new ZebbleBillingServicesBuilder(services, zebbleBillingConfig));

            return services;
        }

        public static IServiceCollection AddStoreConnector<TType>(this IServiceCollection services, string name) where TType : class, IStoreConnector
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            if (name.IsEmpty()) throw new ArgumentNullException(nameof(name));

            services.AddScoped(sp =>
            {
                if (sp.GetServices<StoreConnectorRegistry>().Any(x => x.Name == name))
                    throw new InvalidOperationException($"There is another {nameof(StoreConnectorRegistry)} with name '{name}' in the container.");

                return new StoreConnectorRegistry(name, typeof(TType));
            });

            services.AddScoped<IStoreConnector, TType>();

            return services;
        }
    }
}
