namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Olive;

    partial class BillingContext
    {
        public async Task<decimal> GetPrice(string productId) => (await ProductProvider.GetById(productId)).Price;

        public async Task UpdateProductPrices()
        {
            await UIContext.AwaitConnection(10);
            await Task.Delay(3.Seconds());

            try { await new ProductsPriceUpdaterCommand().Execute(); }
            catch (Exception ex) { Log.For(typeof(BillingContext)).Error(ex); }
        }
    }
}
