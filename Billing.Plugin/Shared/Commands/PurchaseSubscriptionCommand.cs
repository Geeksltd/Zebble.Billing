namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Plugin.InAppBilling;
    using Olive;

    class PurchaseSubscriptionCommand : StoreCommandBase<PurchaseResult>
    {
        readonly Product Product;

        public PurchaseSubscriptionCommand(Product product) => Product = product;

        protected override async Task<PurchaseResult> DoExecute()
        {
            var context = BillingContext.Current;

            try
            {
                await context.Refresh();

                if (context.IsSubscribed && (await context.CurrentProduct)?.Id == Product.Id) return PurchaseResult.AlreadySubscribed;

#if CAFEBAZAAR && ANDROID
                var purchase = await Billing.PurchaseAsync(Product.Id, Product.GetItemType(), context.User.UserId);
#else
                var purchase = await Billing.PurchaseAsync(Product.Id, Product.GetItemType());
#endif

                var result = await context.PurchaseAttempt(purchase.ToEventArgs());
                if (result?.Status != PurchaseAttemptStatus.Succeeded)
                {
                    // In iOS, we get this error when we try to purchase an item that is already associated with another app-specific account
                    if (result?.Status == PurchaseAttemptStatus.UserMismatched) return PurchaseResult.UserMismatched;

                    return PurchaseResult.NotCompleted;
                }

#if !(CAFEBAZAAR && ANDROID)
                if (purchase.State == PurchaseState.Purchased)
                    await Billing.AcknowledgePurchaseAsync(purchase.PurchaseToken);
#endif

                await context.Refresh();

                if (context.IsSubscribed) return PurchaseResult.Succeeded;

                if (purchase.State.IsAnyOf(PurchaseState.Restored, PurchaseState.Purchased, PurchaseState.Purchasing, PurchaseState.PaymentPending))
                    return PurchaseResult.WillBeActivated;

                if (purchase.State.IsAnyOf(PurchaseState.Failed, PurchaseState.Canceled)) return PurchaseResult.NotCompleted;

                return PurchaseResult.Unknown;
            }
            catch (InAppBillingPurchaseException ex)
            {
                Log.For(this).Error(ex);

                return ex.PurchaseError switch
                {
                    PurchaseError.AppStoreUnavailable => PurchaseResult.AppStoreUnavailable,
                    PurchaseError.BillingUnavailable => PurchaseResult.BillingUnavailable,
                    PurchaseError.PaymentInvalid => PurchaseResult.PaymentInvalid,
                    PurchaseError.PaymentNotAllowed => PurchaseResult.PaymentNotAllowed,
#if !(CAFEBAZAAR && ANDROID)
                    PurchaseError.AlreadyOwned => PurchaseResult.AlreadySubscribed,
#endif
                    PurchaseError.UserCancelled => PurchaseResult.UserCancelled,
                    _ => PurchaseResult.Unknown,
                };
            }
            catch (Exception ex)
            {
                Log.For(this).Error(ex);
                return PurchaseResult.Unknown;
            }
        }
    }
}