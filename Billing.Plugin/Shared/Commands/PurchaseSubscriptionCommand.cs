namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Plugin.InAppBilling;
    using Olive;

    class PurchaseSubscriptionCommand : StoreCommandBase<string>
    {
        const string NotCompleted = "Purchase was not completed. Please try again.";
        const string GeneralError = "There seems to be a problem in the subscription system. Please try again later.";
        const string SuccessMessage = "Thank you. Your subscription will be activated as soon as the payment has been received.";
        const string BillingUnavailable = "Billing seems to be unavailable, please try again or contact us.";
        const string PaymentInvalid = "Payment seems to be invalid, please try again.";
        const string PaymentNotAllowed = "Payment does not seem to be enabled/allowed, please try again.";
        const string AppStoreUnavailable = "The app store seems to be unavailable. Try again later.";
        const string Cancelled = "Cancelled";
        const string OK = "OK";

        readonly Product Product;

        public PurchaseSubscriptionCommand(Product product) => Product = product;

        protected override async Task<string> DoExecute()
        {
            try
            {
                var purchase = await Billing.PurchaseAsync(Product.Id, Product.ItemType, new PurchaseVerificator());

                if (purchase == null) return NotCompleted;

                await BillingContext.Current.PurchaseAttempt(purchase.ToEventArgs());

                if (await BillingContext.Current.RestoreSubscription()) return OK;

                if (purchase.State.IsAnyOf(PurchaseState.Restored, PurchaseState.Purchased, PurchaseState.Purchasing, PurchaseState.PaymentPending))
                    return SuccessMessage;

                if (purchase.State.IsAnyOf(PurchaseState.Failed, PurchaseState.Canceled))
                    return NotCompleted;

                return BillingUnavailable;
            }
            catch (InAppBillingPurchaseException ex)
            {
                Log.For(this).Error(ex);

                if (await BillingContext.Current.RestoreSubscription()) return OK;

                return ex.PurchaseError switch
                {
                    PurchaseError.AppStoreUnavailable => AppStoreUnavailable,
                    PurchaseError.BillingUnavailable => BillingUnavailable,
                    PurchaseError.PaymentInvalid => PaymentInvalid,
                    PurchaseError.PaymentNotAllowed => PaymentNotAllowed,
                    PurchaseError.UserCancelled => Cancelled,
                    _ => GeneralError,
                };
            }
            catch (Exception ex)
            {
                Log.For(this).Error(ex);
                return GeneralError;
            }
        }
    }
}