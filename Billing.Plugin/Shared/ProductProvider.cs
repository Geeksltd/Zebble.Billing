namespace Zebble.Billing
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Olive;

    class ProductProvider : IProductProvider
    {
        readonly FileInfo File;
        readonly CatalogOptions Options;

        public ProductProvider(string catalogPath)
        {
            File = catalogPath.IsEmpty() ? throw new ArgumentNullException(nameof(catalogPath)) : Device.IO.File(catalogPath);
            Options = JsonConvert.DeserializeObject<CatalogOptions>(File.ReadAllText());
        }

        public Task<Product> GetById(string productId)
        {
            return GetProducts().FirstOrDefault(x => x.Id == productId);
        }

        public Task<Product[]> GetProducts()
        {
            return Task.FromResult(Options.Products.ToArray());
        }
    }
}
