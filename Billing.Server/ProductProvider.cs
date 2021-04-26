namespace Zebble.Billing
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Olive;

    class ProductProvider : IProductProvider
    {
        readonly CatalogOptions Options;

        public ProductProvider(IOptionsSnapshot<CatalogOptions> options)
        {
            Options = options.Value;
        }

        public Task<Product[]> GetProducts()
        {
            return Task.FromResult(Options.Products.ToArray());
        }

        public Task<Product> GetById(string productId)
        {
            return GetProducts().FirstOrDefault(x => x.Id == productId);
        }
    }
}
