namespace Zebble.Billing
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    static partial class ServiceCollectionExtensions
    {
        static IServiceCollection AddZebbleProductsCache(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<CatalogOptions>(opts => config.GetSection("Catalog")?.Bind(opts));
            services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }
    }
}
