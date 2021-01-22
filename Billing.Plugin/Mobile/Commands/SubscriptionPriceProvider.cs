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

    class SubscriptionPriceProvider : SubscriptionCommand<bool>
    {
        protected override async Task<bool> DoExecute()
        {
            try
            {
                var productGroups = ProductsCache.RegisteredProducts.GroupBy(x => x.Type)
                    .Select(x => new { Type = x.Key, ProductIds = x.Select(p => p.Id).ToArray() });

                foreach (var group in productGroups)
                {
                    var items = await Billing.GetProductInfoAsync(group.Type, group.ProductIds);
                    if (items == null)
                        throw new Exception($"No product info was retrieved for {group.ProductIds.ToString(", ")} ({group.Type})");

                    foreach (var item in items)
                        item.GetProduct()?.UpdatePrice(item.MicrosPrice / 1000000m);
                }

                ProductsCache.SavePrices();

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