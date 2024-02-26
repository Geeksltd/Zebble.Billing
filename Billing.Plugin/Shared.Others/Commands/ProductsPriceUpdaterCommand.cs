namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Plugin.InAppBilling;
    using Olive;
    using System.Linq;

    class ProductsPriceUpdaterCommand : StoreCommandBase<bool>
    {
        const int MaxRetryCount = 3;

        protected override async Task<bool> DoExecute(IBillingUser user)
        {
            try
            {
                var productProvider = BillingContext.Current.ProductProvider;
                var products = productProvider.GetProducts();

                return await ProcessProducts(products).ConfigureAwait(false);
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
            var productProvider = BillingContext.Current.ProductProvider;

            foreach (var product in products)
            {
                var retryCount = 0;

                while (retryCount < MaxRetryCount)
                {
                    try
                    {
                        var item = await Billing
                            .GetProductInfoAsync(product.GetItemType(), product.Id)
                            .FirstOrDefault()
                            .ConfigureAwait(false);

                        if (item == null)
                        {
                            Log.For(this).Warning($"No product info was retrieved for {product.Id}");
                            break;
                        }

                        var discountedMicrosPrice = GetDiscountedPrice(item) ?? default;
                        if (discountedMicrosPrice == default) discountedMicrosPrice = item.MicrosPrice;

                        await productProvider.UpdatePrice(
                            item.ProductId,
                            item.MicrosPrice,
                            discountedMicrosPrice, item.CurrencyCode).ConfigureAwait(false);

                        break;
                    }
                    catch (InAppBillingPurchaseException ex)
                    when (ex.PurchaseError.IsAnyOf(
                        #if CAFEBAZAAR == false
                        PurchaseError.ServiceDisconnected,
                        PurchaseError.ServiceTimeout,
                        #endif
                        PurchaseError.ServiceUnavailable,
                        PurchaseError.BillingUnavailable,
                        PurchaseError.AppStoreUnavailable,
                        PurchaseError.GeneralError))
                    {
                        retryCount++;
                        await Task.Delay(100.Milliseconds());

#if CAFEBAZAAR == false
                        // In some cases, the billing client gets disconnected
                        // So we need to ensure we're still alive
                        if (Billing.IsConnected == false)
                            await Billing.ConnectAsync().ConfigureAwait(false);
#endif
                    }
                }
            }

            return true;
        }

        static decimal? GetDiscountedPrice(InAppBillingProduct item)
        {
#if ANDROID
#if CAFEBAZAAR
            return null;
#else
            return item.AndroidExtras?
                .SubscriptionOfferDetails?
                .SelectMany(x => x.PricingPhases)
                .ExceptNull()
                .MinOrNull(x => x.PriceAmountMicros);
#endif
#elif IOS
            return (decimal?)item.AppleExtras?
                .Discounts.OrEmpty()
                .Concat(item.AppleExtras.IntroductoryOffer)
                .ExceptNull()
                .MinOrNull(x => x.Price);
#else
            // return item.WindowsExtras?.FormattedBasePrice;
            return null;
#endif
        }
    }
}