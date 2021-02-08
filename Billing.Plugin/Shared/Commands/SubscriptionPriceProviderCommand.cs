namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
#if CAFEBAZAAR
    using Plugin.InAppBilling.Abstractions;
#endif
    using Plugin.InAppBilling;
    using Olive;
    using System.Linq;

    class SubscriptionPriceProviderCommand : StoreCommandBase<bool>
    {
        protected override async Task<bool> DoExecute()
        {
            try
            {
                var productProvider = BillingContext.ProductProvider;
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