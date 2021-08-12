namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Olive;

    partial class BillingContext
    {
        /// <summary>
        /// Gets the list of the predefined products.
        /// </summary>
        public Task<Product[]> GetProducts() => ProductProvider.GetProducts();

        /// <summary>
        /// Gets a product by its Id.
        /// </summary>
        public async Task<Product> GetProduct(string productId)
        {
            if (productId.IsEmpty()) return null;

            var product = await ProductProvider.GetById(productId) ?? new Product
            {
                Id = productId,
                Type = ProductType.Voucher
            };

            return product;
        }

        /// <summary>
        /// Gets a product's price by its Id.
        /// </summary>
        public async Task<decimal> GetPrice(string productId) => (await GetProduct(productId)).Price;

        /// <summary>
        /// Gets a product's local price by its Id.
        /// </summary>
        public async Task<string> GetLocalPrice(string productId) => (await GetProduct(productId)).LocalPrice;

        /// <summary>
        /// Fetches and stores the latest prices from the store.
        /// </summary>
        /// <remarks>An active internet connection is required.</remarks>
        public async Task UpdateProductPrices()
        {
            var pricesUpdated = false;

#if !MVVM && !UWP
            await UIContext.AwaitConnection(10);
            await Task.Delay(3.Seconds());

            try { pricesUpdated = await new ProductsPriceUpdaterCommand().Execute(); }
            catch (Exception ex) { Log.For(typeof(BillingContext)).Error(ex); }
#endif

            if (pricesUpdated) return;
            await PriceUpdateFailed.Raise(new PriceUpdateFailedEventArgs());
        }
    }
}
