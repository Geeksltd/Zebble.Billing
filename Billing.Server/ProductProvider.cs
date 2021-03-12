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

        public Task<Product[]> GetProducts(string platform)
        {
            return Task.FromResult(Options.Products.Where(x => x.Platform.IsEmpty() || x.Platform.Equals(platform, false)).ToArray());
        }

        public Task<Product> GetById(string platform, string productId)
        {
            return GetProducts(platform).FirstOrDefault(x => x.Id == productId);
        }
    }
}
