namespace Zebble.Billing
{
    using System.Linq;
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

        public Product[] GetProducts()
        {
            return Options.Products.ToArray();
        }

        public Product GetById(string productId)
        {
            var product = GetProducts().FirstOrDefault(x => x.Id == productId);
            if (product is null) Logger.LogWarning($"No product with id '{productId}' found.");
            return product;
        }
    }
}
