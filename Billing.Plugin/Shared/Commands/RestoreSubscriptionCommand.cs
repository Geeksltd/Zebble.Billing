namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Plugin.InAppBilling;
    using Olive;

    class RestoreSubscriptionCommand : StoreCommandBase<bool>
    {
        protected override async Task<bool> DoExecute()
        {
            return await Fetch(ItemType.Subscription) | await Fetch(ItemType.InAppPurchase);
        }

        async Task<bool> Fetch(ItemType type)
        {
            try
            {
                var purchases = await Billing.GetPurchasesAsync(type);
                if (purchases.None()) return false;

                foreach (var purchase in purchases.Distinct(x => x.Id))
                {
                    if (purchase.State != PurchaseState.Purchased) continue;
#if !(CAFEBAZAAR && ANDROID)
                    if (purchase.IsAcknowledged) continue;
#endif

                    await BillingContext.Current.PurchaseAttempt(purchase.ToEventArgs());
#if !(CAFEBAZAAR && ANDROID)
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