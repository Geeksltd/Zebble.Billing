namespace Zebble.Billing
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Olive;

    static partial class ServiceCollectionExtensions
    {
        static IServiceCollection AddZebbleProductsCache(this IServiceCollection services)
        {
            services.AddOptions<CatalogOptions>()
                    .Configure<IConfiguration>((opts, config) => config.GetSection("ZebbleBilling:Catalog")?.Bind(opts))
                    .Validate(opts => opts.Products == null, $"{nameof(CatalogOptions.Products)} is null.")
                    .Validate(opts => opts.Products.None(), $"{nameof(CatalogOptions.Products)} is empty.");

            services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }
    }
}
