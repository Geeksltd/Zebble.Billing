namespace Zebble.Billing
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
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

		public Task<Product> GetById(string productId)
		{
			return GetProducts().SingleOrDefault(x => x.Id == productId);
		}

		public Task<Product[]> GetProducts()
		{
			return Task.FromResult(Options.Products.Where(x => x.Platform.IsEmpty() || x.Platform.Equals(BillingContext.PaymentAuthority, false)).ToArray());
		}

		public async Task UpdatePrice(string productId, decimal microsPrice, string currencyCode)
		{
			var product = await GetById(productId);
			if (product is null)
			{
				Log.For(this).Error($"No product with id '{productId}' found.");
				return;
			}

			var rawPrice = microsPrice / 1000000m;
			var price = Math.Round(rawPrice, 2);

			if (product.Price == price)
			{
				Log.For(this).Info($"The price of the product with id '{productId}' isn't changed since the last update. ({product.Price} - {product.LocalPrice})");
				return;
			}

			product.Price = price;
			product.CurrencySymbol = CurrencyTools.GetCurrencySymbol(currencyCode);

			await File.WriteAllTextAsync(JsonConvert.SerializeObject(Options));
		}

		static class CurrencyTools
		{
			static readonly IDictionary<string, string> Cache;

			static CurrencyTools()
			{
				Cache = CultureInfo
					.GetCultures(CultureTypes.AllCultures)
					.Where(x => !x.IsNeutralCulture)
					.Select(x => x.Name)
					.Where(x => x.HasValue())
					.Select(x => new RegionInfo(x))
					.GroupBy(x => x.ISOCurrencySymbol)
					.ToDictionary(x => x.Key, x => x.First().CurrencySymbol);
			}

			public static string GetCurrencySymbol(string code)
			{
				return Cache.TryGetValue(code, out var symbol) ? symbol : code;
			}
		}
	}
}
