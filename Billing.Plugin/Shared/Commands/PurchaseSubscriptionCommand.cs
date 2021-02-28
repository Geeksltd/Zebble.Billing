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
            try
            {
#if CAFEBAZAAR && ANDROID
                var purchase = await Billing.PurchaseAsync(Product.Id, Product.ItemType, BillingContext.Current.User.UserId, new PurchaseVerificator());
#else
                var purchase = await Billing.PurchaseAsync(Product.Id, Product.ItemType, new PurchaseVerificator());
#endif

                if (purchase == null) return PurchaseResult.NotCompleted;

                await BillingContext.Current.PurchaseAttempt(purchase.ToEventArgs());

                await BillingContext.Current.Refresh();

                if (BillingContext.Current.IsSubscribed) return PurchaseResult.Succeeded;

                if (purchase.State.IsAnyOf(PurchaseState.Restored, PurchaseState.Purchased, PurchaseState.Purchasing, PurchaseState.PaymentPending))
                    return PurchaseResult.WillBeActivated;

                if (purchase.State.IsAnyOf(PurchaseState.Failed, PurchaseState.Canceled)) return PurchaseResult.NotCompleted;

                return PurchaseResult.BillingUnavailable;
            }
            catch (InAppBillingPurchaseException ex)
            {
                Log.For(this).Error(ex);

                if (await BillingContext.Current.RestoreSubscription()) return PurchaseResult.Succeeded;

                return ex.PurchaseError switch
                {
                    PurchaseError.AppStoreUnavailable => PurchaseResult.AppStoreUnavailable,
                    PurchaseError.BillingUnavailable => PurchaseResult.BillingUnavailable,
                    PurchaseError.PaymentInvalid => PurchaseResult.PaymentInvalid,
                    PurchaseError.PaymentNotAllowed => PurchaseResult.PaymentNotAllowed,
                    PurchaseError.UserCancelled => PurchaseResult.Cancelled,
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