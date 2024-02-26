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
        public Product[] GetProducts() => ProductProvider.GetProducts();

        /// <summary>
        /// Gets a product by its Id.
        /// </summary>
        public Product GetProduct(string productId)
        {
            return ProductProvider.GetById(productId) ?? new Product
            {
                Id = productId,
                Type = ProductType.Voucher
            };
        }

        /// <summary>
        /// Gets a product's original price by its Id.
        /// </summary>
        public decimal GetOriginalPrice(string productId) => GetProduct(productId).OriginalPrice;

        /// <summary>
        /// Gets a product's local original price by its Id.
        /// </summary>
        public string GetLocalOriginalPrice(string productId) => GetProduct(productId).LocalOriginalPrice;

        /// <summary>
        /// Gets a product's discounted price by its Id.
        /// </summary>
        public decimal GetDiscountedPrice(string productId) => GetProduct(productId).DiscountedPrice;

        /// <summary>
        /// Gets a product's local discounted price by its Id.
        /// </summary>
        public string GetLocalDiscountedPrice(string productId) => GetProduct(productId).LocalDiscountedPrice;

        /// <summary>
        /// Fetches and stores the latest prices from the store.
        /// </summary>
        /// <remarks>An active internet connection is required.</remarks>
        public async Task UpdateProductPrices(IBillingUser user)
        {
            var pricesUpdated = false;

#if !MVVM && !UWP
            await UIContext.AwaitConnection(10).ConfigureAwait(false);
            await Task.Delay(3.Seconds()).ConfigureAwait(false);

            try { pricesUpdated = await new ProductsPriceUpdaterCommand().Execute(user).ConfigureAwait(false); }
            catch (System.Exception ex) { Log.For(typeof(BillingContext)).Error(ex); }
#endif

            if (pricesUpdated) await PriceUpdated.Raise().ConfigureAwait(false);
            else await PriceUpdateFailed.Raise(new PriceUpdateFailedEventArgs
            {
                ProductIds = ProductProvider.GetProducts().Select(x => x.Id).ToArray(),
                UpdatePrice = args => ProductProvider.UpdatePrice(args.ProductId, args.OriginalMicrosPrice, args.DiscountedMicrosPrice, args.CurrencyCode),
            }).ConfigureAwait(false);
        }
    }
}
