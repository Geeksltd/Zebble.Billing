namespace Zebble.Billing
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Plugin.InAppBilling;
    using Olive;

    class RestoreSubscriptionCommand : StoreCommandBase<bool>
    {
        protected override async Task<bool> DoExecute()
        {
            try
            {
                var subscriptions = await Billing.GetPurchasesAsync(ItemType.Subscription).ToArray();
                var inAppPurchases = await Billing.GetPurchasesAsync(ItemType.InAppPurchase).ToArray();

                var purchases = subscriptions.Concat(inAppPurchases)
                                             .Where(x => x.State == PurchaseState.Purchased)
                                             .Where(x => x.PurchaseToken.HasValue())
                                             .Distinct(x => new { x.Id, x.ProductId }).ToArray();

                if (purchases.None()) return false;

                foreach (var purchase in purchases)
                {
                    var (result, _) = await BillingContext.Current.ProcessPurchase(purchase);

#if !(CAFEBAZAAR && ANDROID)
                    if (result != PurchaseResult.Succeeded) continue;
                    await Billing.AcknowledgePurchaseAsync(purchase.PurchaseToken);
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
    }
}