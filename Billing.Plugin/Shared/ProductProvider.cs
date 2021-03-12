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

        public Task<Product> GetById(string platform, string productId)
        {
            return GetProducts(platform).SingleOrDefault(x => x.Id == productId);
        }

        public Task<Product[]> GetProducts(string platform)
        {
            return Task.FromResult(Options.Products.Where(x => x.Platform.IsEmpty() || x.Platform.Equals(platform, false)).ToArray());
        }

        public async Task UpdatePrice(string productId, decimal price)
        {
            Options.Products.Single(
                x => x.Id == productId
            ).Price = price;

            await File.WriteAllTextAsync(JsonConvert.SerializeObject(Options));
        }
    }
}
