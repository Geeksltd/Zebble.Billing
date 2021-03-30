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

                if (context.IsSubscribed && context.CurrentProductId == Product.Id) return PurchaseResult.AlreadySubscribed;

                var verificator = new PurchaseVerificator();

#if CAFEBAZAAR && ANDROID
                var purchase = await Billing.PurchaseAsync(Product.Id, Product.ItemType, context.User.UserId, verificator);
#else
                var purchase = await Billing.PurchaseAsync(Product.Id, Product.ItemType, verificator);
#endif

                if (purchase is null)
                {
                    if (verificator.Status == PurchaseVerificationResult.UserMismatched) return PurchaseResult.UserMismatched;

                    return PurchaseResult.NotCompleted;
                }

                await context.PurchaseAttempt(purchase.ToEventArgs());

                await context.Refresh();

                if (context.IsSubscribed) return PurchaseResult.Succeeded;

                if (purchase.State.IsAnyOf(PurchaseState.Restored, PurchaseState.Purchased, PurchaseState.Purchasing, PurchaseState.PaymentPending))
                    return PurchaseResult.WillBeActivated;

                if (purchase.State.IsAnyOf(PurchaseState.Failed, PurchaseState.Canceled)) return PurchaseResult.NotCompleted;

                return PurchaseResult.BillingUnavailable;
            }
            catch (InAppBillingPurchaseException ex)
            {
                Log.For(this).Error(ex);

                if (await context.RestoreSubscription()) return PurchaseResult.Succeeded;

                return ex.PurchaseError switch
                {
                    PurchaseError.AppStoreUnavailable => PurchaseResult.AppStoreUnavailable,
                    PurchaseError.BillingUnavailable => PurchaseResult.BillingUnavailable,
                    PurchaseError.PaymentInvalid => PurchaseResult.PaymentInvalid,
                    PurchaseError.PaymentNotAllowed => PurchaseResult.PaymentNotAllowed,
                    PurchaseError.UserCancelled => PurchaseResult.UserCancelled,
                    _ => PurchaseResult.GeneralError,
                };
            }
            catch (Exception ex)
            {
                Log.For(this).Error(ex);
                return PurchaseResult.GeneralError;
            }
        }
    }
}