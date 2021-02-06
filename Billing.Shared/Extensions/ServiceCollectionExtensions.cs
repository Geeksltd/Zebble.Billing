namespace Zebble.Billing
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    static partial class ServiceCollectionExtensions
    {
        static IServiceCollection AddZebbleProductsCache(this IServiceCollection services, IConfiguration config)
        {
            services.AddOptions<CatalogOptions>()
                    .Configure(opts => config.GetSection("Catalog")?.Bind(opts))
                    .Validate(opts => opts.Validate());

            services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }
    }
}
