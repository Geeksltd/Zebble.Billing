namespace Zebble.Billing
{
    using System;
    using System.Linq;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Olive;

    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddZebbleBilling(this IServiceCollection services, Action<ZebbleBillingServicesBuilder> builder = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddOptions<CatalogOptions>()
                    .Configure<IConfiguration>((opts, config) => config.GetSection("ZebbleBilling:Catalog")?.Bind(opts))
                    .Validate(opts => opts.Products is not null, $"{nameof(CatalogOptions.Products)} is null.")
                    .Validate(opts => opts.Products.Any(), $"{nameof(CatalogOptions.Products)} is empty.");

            services.AddScoped<IProductProvider, ProductProvider>();

            services.AddScoped<IStoreConnectorResolver, StoreConnectorResolver>();

            services.AddScoped<SubscriptionManager>();

            builder?.Invoke(new ZebbleBillingServicesBuilder(services));

            return services;
        }

        public static IServiceCollection AddStoreConnector<TType>(this IServiceCollection services, string name) where TType : class, IStoreConnector
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            if (name.IsEmpty()) throw new ArgumentNullException(nameof(name));

            services.AddScoped(sp => new StoreConnectorRegistry(name, typeof(TType)));

            services.AddScoped<IStoreConnector, TType>();

            return services;
        }
    }
}
