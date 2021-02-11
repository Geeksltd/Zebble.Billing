namespace Zebble.Billing
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Olive;

    class ProductProvider<T> : IProductProvider<T> where T: Product
    {
        readonly FileInfo File;
        readonly CatalogOptions<T> Options;

        public ProductProvider(string catalogPath)
        {
            File = catalogPath.IsEmpty() ? throw new ArgumentNullException(nameof(catalogPath)) : Device.IO.File(catalogPath);
            Options = JsonConvert.DeserializeObject<CatalogOptions<T>>(File.ReadAllText());
        }

        public Task<T> GetById(string productId)
        {
            return GetProducts().SingleOrDefault(x => x.Id == productId) ?? throw new Exception($"Product with id '{productId}' not found.");
        }

        public Task<T[]> GetProducts() => Task.FromResult(Options.Products.ToArray());

        public async Task UpdatePrice(string productId, decimal price)
        {
            Options.Products.Single(
                x => x.Id == productId
            ).Price = price;

            await File.WriteAllTextAsync(JsonConvert.SerializeObject(Options));
        }
    }
}
