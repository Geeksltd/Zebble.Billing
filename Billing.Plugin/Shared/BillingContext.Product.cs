namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Olive;

    partial class BillingContext
    {
        public Task<Product[]> GetProducts() => ProductProvider.GetProducts();

        public Task<Product> GetProduct(string productId) => ProductProvider.GetById(productId);

        public async Task<decimal> GetPrice(string productId) => (await GetProduct(productId)).Price;

        public async Task UpdateProductPrices()
        {
            await UIContext.AwaitConnection(10);
            await Task.Delay(3.Seconds());

            try { await new ProductsPriceUpdaterCommand().Execute(); }
            catch (Exception ex) { Log.For(typeof(BillingContext)).Error(ex); }
        }
    }
}
