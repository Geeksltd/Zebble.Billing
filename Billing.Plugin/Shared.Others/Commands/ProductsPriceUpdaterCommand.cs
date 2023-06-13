namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Plugin.InAppBilling;
    using Olive;
    using System.Linq;

    class ProductsPriceUpdaterCommand : StoreCommandBase<bool>
    {
        protected override async Task<bool> DoExecute(IBillingUser user)
        {
            try
            {
                var productProvider = BillingContext.Current.ProductProvider;
                var products = await productProvider.GetProducts();

                return await ProcessProducts(products);
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

        async Task<bool> ProcessProducts(Product[] products)
        {
            try
            {
                var productProvider = BillingContext.Current.ProductProvider;
                var groups = products.GroupBy(x => x.GetItemType())
                    .Select(x => new { ItemType = x.Key, ProductIds = x.Select(p => p.Id).ToArray() });

                foreach (var group in groups)
                {
                    var items = await Billing.GetProductInfoAsync(group.ItemType, group.ProductIds);
                    if (items == null)
                        throw new Exception($"No product info was retrieved for {group.ProductIds.ToString(", ")} ({group.ItemType})");

                    foreach (var item in items)
                    {
                        var discountedMicrosPrice = GetDiscountedPrice(item) ?? default;
                        if (discountedMicrosPrice == default) discountedMicrosPrice = item.MicrosPrice;

                        await productProvider.UpdatePrice(item.ProductId, item.MicrosPrice, discountedMicrosPrice, item.CurrencyCode);
                    }
                }

                return true;
            }
            catch (InAppBillingPurchaseException ex)
            {
                if (ex.PurchaseError == PurchaseError.InvalidProduct)
                {
                    // Invalid Product: XYZ
                    var invalidProductId = ex.Message.Split(":").Last().Trim();
                    var filteredProducts = products.Except(x => x.Id.Equals(invalidProductId, caseSensitive: false)).ToArray();
                    return await ProcessProducts(filteredProducts);
                }

                throw;
            }
        }

        static decimal? GetDiscountedPrice(InAppBillingProduct item)
        {
#if ANDROID
#if CAFEBAZAAR
            return null;
#else
            return item.AndroidExtras?.MicrosIntroductoryPrice;
#endif
#elif IOS
            return (decimal?)item.AppleExtras?.Discounts.OrEmpty().Concat(item.AppleExtras.IntroductoryOffer).ExceptNull().MinOrNull(x => x.Price);
#else
            // return item.WindowsExtras?.FormattedBasePrice;
            return null;
#endif
        }
    }
}