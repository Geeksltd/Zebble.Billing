namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Plugin.InAppBilling;
    using Olive;

    class PurchaseSubscriptionCommand : StoreCommandBase<(PurchaseResult, string)>
    {
        readonly Product Product;

        public PurchaseSubscriptionCommand(Product product) => Product = product;

        protected override async Task<(PurchaseResult, string)> DoExecute()
        {
            var context = BillingContext.Current;

            try
            {
                await context.Refresh();

                if (context.IsSubscribed && (await context.CurrentProduct)?.Id == Product.Id) return (PurchaseResult.AlreadySubscribed, null);

#if CAFEBAZAAR && ANDROID
                var purchase = await Billing.PurchaseAsync(Product.Id, Product.GetItemType(), context.User.UserId);
#else
                var purchase = await Billing.PurchaseAsync(Product.Id, Product.GetItemType());
#endif

                var result = await context.PurchaseAttempt(purchase.ToEventArgs());
                if (result?.Status != PurchaseAttemptStatus.Succeeded)
                {
                    // In iOS, we get this error when we try to purchase an item that is already associated with another app-specific account
                    // And BillingOptions.UserMismatchResolvingStrategy is set to block
                    if (result?.Status == PurchaseAttemptStatus.UserMismatchedAndBlocked)
                        return (PurchaseResult.UserMismatchedAndBlocked, result.OriginUserId);

                    // In iOS, we get this error when we try to purchase an item that is already associated with another app-specific account
                    // And BillingOptions.UserMismatchResolvingStrategy is set to replace
                    // The developer should consider this as a successful purchase
                    if (result?.Status == PurchaseAttemptStatus.UserMismatchedAndReplaced)
                        return (PurchaseResult.UserMismatchedAndReplaced, result.OriginUserId);

                    return (PurchaseResult.NotCompleted, null);
                }

#if !(CAFEBAZAAR && ANDROID)
                if (purchase.State == PurchaseState.Purchased)
                    await Billing.AcknowledgePurchaseAsync(purchase.PurchaseToken);
#endif

                await context.Refresh();

                if (context.IsSubscribed) return (PurchaseResult.Succeeded, null);

                if (purchase.State.IsAnyOf(PurchaseState.Restored, PurchaseState.Purchased, PurchaseState.Purchasing, PurchaseState.PaymentPending))
                    return (PurchaseResult.WillBeActivated, null);

                if (purchase.State.IsAnyOf(PurchaseState.Failed, PurchaseState.Canceled)) return (PurchaseResult.NotCompleted, null);

                return (PurchaseResult.Unknown, null);
            }
            catch (InAppBillingPurchaseException ex)
            {
                Log.For(this).Error(ex);

                return ex.PurchaseError switch
                {
                    PurchaseError.AppStoreUnavailable => (PurchaseResult.AppStoreUnavailable, null),
                    PurchaseError.BillingUnavailable => (PurchaseResult.BillingUnavailable, null),
                    PurchaseError.PaymentInvalid => (PurchaseResult.PaymentInvalid, null),
                    PurchaseError.PaymentNotAllowed => (PurchaseResult.PaymentNotAllowed, null),
#if !(CAFEBAZAAR && ANDROID)
                    PurchaseError.AlreadyOwned => (PurchaseResult.AlreadySubscribed, null),
#endif
                    PurchaseError.UserCancelled => (PurchaseResult.UserCancelled, null),
                    _ => (PurchaseResult.Unknown, null),
                };
            }
            catch (Exception ex)
            {
                Log.For(this).Error(ex);
                return (PurchaseResult.Unknown, null);
            }
        }
    }
}