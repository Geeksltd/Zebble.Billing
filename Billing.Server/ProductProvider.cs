namespace Zebble.Billing
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Olive;

    class ProductProvider : IProductProvider
    {
        readonly ILogger<ProductProvider> Logger;
        readonly CatalogOptions Options;

        public ProductProvider(ILogger<ProductProvider> logger, IOptionsSnapshot<CatalogOptions> options)
        {
            Options = options.Value;
            Logger = logger;
        }

        public Task<Product[]> GetProducts()
        {
            return Task.FromResult(Options.Products.ToArray());
        }

        public async Task<Product> GetById(string productId)
        {
            var product = await GetProducts().FirstOrDefault(x => x.Id == productId);
            if (product is null) Logger.LogWarning($"No product with id '{productId}' found.");
            return product;
        }
    }
}
