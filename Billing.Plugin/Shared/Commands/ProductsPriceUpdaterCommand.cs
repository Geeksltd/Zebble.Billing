namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Plugin.InAppBilling;
    using Olive;
    using System.Linq;

    class ProductsPriceUpdaterCommand<T> : StoreCommandBase<bool> where T : Product
    {
        protected override async Task<bool> DoExecute()
        {
            try
            {
                var productProvider = BillingContext<T>.Current.ProductProvider;
                var products = await productProvider.GetProducts();
                var groups = products.GroupBy(x => x.ItemType)
                    .Select(x => new { ItemType = x.Key, ProductIds = x.Select(p => p.Id).ToArray() });

                foreach (var group in groups)
                {
                    var items = await Billing.GetProductInfoAsync(group.ItemType, group.ProductIds);
                    if (items == null)
                        throw new Exception($"No product info was retrieved for {group.ProductIds.ToString(", ")} ({group.ItemType})");

                    foreach (var item in items)
                        await productProvider.UpdatePrice(item.ProductId, item.MicrosPrice / 1000000m);
                }

                return true;
            }
            catch (InAppBillingPurchaseException ex)
            {
                Log.For(this).Error(ex);
                throw;
            }
            catch (Exception ex)
            {
                Log.For(this).Error(ex);
                throw;
            }
        }
    }
}