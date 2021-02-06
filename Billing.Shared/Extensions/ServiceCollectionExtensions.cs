namespace Zebble.Billing
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    static partial class ServiceCollectionExtensions
    {
        static IServiceCollection AddZebbleProductsCache(this IServiceCollection services)
        {
            services.AddOptions<CatalogOptions>()
                    .Configure<IConfiguration>((opts, config) => config.GetSection("ZebbleBilling:Catalog")?.Bind(opts))
                    .Validate(opts => opts.Validate());

            services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }
    }
}
