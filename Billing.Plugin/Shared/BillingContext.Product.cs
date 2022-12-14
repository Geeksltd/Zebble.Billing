namespace Zebble.Billing
{
    using System.Linq;
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
            return await ProductProvider.GetById(productId) ?? new Product
            {
                Id = productId,
                Type = ProductType.Voucher
            };
        }

        /// <summary>
        /// Gets a product's original price by its Id.
        /// </summary>
        public async Task<decimal> GetOriginalPrice(string productId) => (await GetProduct(productId)).OriginalPrice;

        /// <summary>
        /// Gets a product's local original price by its Id.
        /// </summary>
        public async Task<string> GetLocalOriginalPrice(string productId) => (await GetProduct(productId)).LocalOriginalPrice;

        /// <summary>
        /// Gets a product's discounted price by its Id.
        /// </summary>
        public async Task<decimal> GetDiscountedPrice(string productId) => (await GetProduct(productId)).DiscountedPrice;

        /// <summary>
        /// Gets a product's local discounted price by its Id.
        /// </summary>
        public async Task<string> GetLocalDiscountedPrice(string productId) => (await GetProduct(productId)).LocalDiscountedPrice;

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
            catch (System.Exception ex) { Log.For(typeof(BillingContext)).Error(ex); }
#endif

            if (pricesUpdated) return;
            await PriceUpdateFailed.Raise(new PriceUpdateFailedEventArgs
            {
                ProductIds = (await ProductProvider.GetProducts()).Select(x => x.Id).ToArray(),
                UpdatePrice = args => ProductProvider.UpdatePrice(args.ProductId, args.OriginalMicrosPrice, args.DiscountedMicrosPrice, args.CurrencyCode),
            });
        }
    }
}
