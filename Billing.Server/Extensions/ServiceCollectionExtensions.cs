namespace Zebble.Billing
{
    using System;
    using System.Linq;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Olive;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddZebbleBilling(
            this IServiceCollection services,
            Action<ZebbleBillingServicesBuilder> builder = null
        )
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddOptions<BillingOptions>()
                    .Configure<IConfiguration>((opts, config) => config.GetSection("ZebbleBilling")?.Bind(opts))
                    .Validate(opts => opts.PurchaseAttemptPath is not null, $"{nameof(BillingOptions.PurchaseAttemptPath)} is null.")
                    .Validate(opts => opts.SubscriptionStatusPath is not null, $"{nameof(BillingOptions.SubscriptionStatusPath)} is null.");

            services.AddOptions<CatalogOptions>()
                    .Configure<IConfiguration>((opts, config) => config.GetSection("ZebbleBilling:Catalog")?.Bind(opts))
                    .Validate(opts => opts.Products is not null, $"{nameof(CatalogOptions.Products)} is null.")
                    .Validate(opts => opts.Products.Any(), $"{nameof(CatalogOptions.Products)} is empty.");

            services.AddScoped<IProductProvider, ProductProvider>();

            services.AddScoped<IStoreConnectorResolver, StoreConnectorResolver>();

            services.TryAddScoped<ISubscriptionManager, DefaultSubscriptionManager>();

            builder?.Invoke(new ZebbleBillingServicesBuilder(services));

            services.TryAddScoped<ISubscriptionComparer, DefaultSubscriptionComparer>();

            services.TryAddScoped<ISubscriptionChangeHandler, DefaultSubscriptionChangeHandler>();

            return services;
        }

        public static IServiceCollection AddStoreConnector<TType>(this IServiceCollection services, string name) where TType : class, IStoreConnector
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            if (name.IsEmpty()) throw new ArgumentNullException(nameof(name));

            services.AddScoped(sp => new StoreConnectorRegistry(name, typeof(TType)));

            services.AddScoped<IStoreConnector, TType>();
            services.AddScoped(sp => sp.GetServices<IStoreConnector>().OfType<TType>().Single());

            return services;
        }
    }
}
