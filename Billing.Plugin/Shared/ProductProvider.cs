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
            if (catalogPath.IsEmpty())
                throw new ArgumentNullException(nameof(catalogPath));

            File = Device.IO.File(catalogPath).ExistsOrThrow();

            var text = File.ReadAllText();

            Options = JsonConvert.DeserializeObject<CatalogOptions>(text);

            if (Options.Products is null) throw new Exception("Products is null in: " + File.Name);
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
