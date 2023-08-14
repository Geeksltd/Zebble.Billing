namespace Zebble.Billing
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Plugin.InAppBilling;
    using Olive;

    class RestoreSubscriptionCommand : StoreCommandBase<bool>
    {
        protected override async Task<bool> DoExecute(IBillingUser user)
        {
            try
            {
                var subscriptions = await GetPurchasesAsync(ProductType.Subscription);
                var inAppPurchases = await GetPurchasesAsync(ProductType.InAppPurchase);

                var ignoringStates = new[]
                {
                    PurchaseState.Canceled,
                    PurchaseState.Failed,
#if (CAFEBAZAAR && ANDROID)
                    PurchaseState.Refunded,
#endif
                    PurchaseState.Unknown
                };

                var purchases = subscriptions.Concat(inAppPurchases)
                                             .Except(x => x.State.IsAnyOf(ignoringStates))
                                             .Where(x => x.PurchaseToken.HasValue())
                                             .Distinct(x => new { x.Id, x.ProductId }).ToArray();

                if (purchases.None()) return false;

                foreach (var purchase in purchases)
                {
                    var (result, _) = await BillingContext.Current.ProcessPurchase(user, purchase);

#if !(CAFEBAZAAR && ANDROID)
                    if (result != PurchaseResult.Succeeded) continue;
                    await Billing.FinalizePurchaseAsync(purchase.PurchaseToken);
#endif
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.For(this).Error(ex);
                return false;
            }
        }

        async Task<InAppBillingPurchase[]> GetPurchasesAsync(ProductType productType)
        {
            return await Billing.GetPurchasesAsync(productType.GetItemType()).ToArray();
        }
    }
}