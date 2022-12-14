namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Plugin.InAppBilling;
    using Olive;
    using System.Linq;

    class ProductsPriceUpdaterCommand : StoreCommandBase<bool>
    {
        protected override async Task<bool> DoExecute()
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
                        await productProvider.UpdatePrice(item.ProductId, item.MicrosPrice, GetDiscountedPrice(item) ?? item.MicrosPrice, item.CurrencyCode);
                }

                return true;
            }
            catch (InAppBillingPurchaseException ex)
            {
                if (ex.PurchaseError == PurchaseError.InvalidProduct)
                {
                    // Invalid Product: XYZ
                    var invalidProductId = ex.Message.Split(":").LastOrDefault().Trim();
                    var filteredProducts = products.Except(x => x.Id.Equals(invalidProductId, caseSensitive: false)).ToArray();
                    return await ProcessProducts(filteredProducts);
                }

                throw;
            }
        }

        decimal? GetDiscountedPrice(InAppBillingProduct item)
        {
            if (BillingContext.PaymentAuthority == "GooglePlay")
                return item.AndroidExtras?.MicrosIntroductoryPrice;

            if (BillingContext.PaymentAuthority == "AppStore")
                return (decimal?)item.AppleExtras.Discounts?.Min(x => x.Price);

            if (BillingContext.PaymentAuthority == "WindowsStore")
                // return item.WindowsExtras?.FormattedBasePrice;
                return null;

            return null;
        }
    }
}