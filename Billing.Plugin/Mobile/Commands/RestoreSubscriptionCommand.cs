namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Plugin.InAppBilling;
    using Olive;

    class RestoreSubscriptionCommand : SubscriptionCommand<bool>
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

                foreach (var purchase in purchases)
                    await BillingContext.PurchaseRecognized.Raise(purchase.ToEventArgs());

                return true;
            }
            catch (InAppBillingPurchaseException purchaseEx)
            {
                Log.For(this).Error(purchaseEx);
                return false;
            }
            catch (Exception ex)
            {
                Log.For(this).Error(ex);
                return false;
            }
        }
    }
}