namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Plugin.InAppBilling;
    using Olive;

    class PurchaseSubscriptionCommand<T> : StoreCommandBase<string> where T : Product
    {
        const string NOT_COMPLETED = "Purchase was not completed. Please try again.";
        const string GeneralError = "There seems to be a problem in the subscription system. Please try again later.";
        const string SuccessMessage = "Thank you. Your Pro will be activated as soon as the payment has been received.";

        static bool StartedTrying;
        readonly Product Product;

        public PurchaseSubscriptionCommand(Product product) => Product = product;

        protected override async Task<string> DoExecute()
        {
            try
            {
                var purchase = await Billing.PurchaseAsync(Product.Id, Product.ItemType, null);

                if (purchase == null) return NOT_COMPLETED;

                await BillingContext<T>.Current.PurchaseAttempt(await purchase.ToEventArgs<T>());

                if (purchase.State.IsAnyOf(PurchaseState.Restored, PurchaseState.Purchased, PurchaseState.Purchasing, PurchaseState.PaymentPending))
                {
                    for (var attempt = 10; attempt > 0; attempt--)
                    {
                        await Task.Delay(100);
                        if (await BillingContext<T>.Current.RestoreSubscription()) return "OK";
                    }

                    Thread.Pool.RunOnNewThread(KeepTrying);
                    return SuccessMessage;
                }
                else if (purchase.State.IsAnyOf(PurchaseState.Failed, PurchaseState.Canceled))
                {
                    if (await BillingContext<T>.Current.RestoreSubscription()) return "OK";

                    return NOT_COMPLETED;
                }

                return "Billing seems to be unavailable, please try again or contact support@wordupapp.co.";
            }
            catch (InAppBillingPurchaseException ex)
            {
                Log.For(this).Error(ex);

                await new RestoreSubscriptionCommand<T>().Execute();

                if (await BillingContext<T>.Current.RestoreSubscription()) return "OK";

                Thread.Pool.RunOnNewThread(KeepTrying);

                switch (ex.PurchaseError)
                {
                    case PurchaseError.AppStoreUnavailable:
                        return "The app store seems to be unavailable. Try again later.";
                    case PurchaseError.BillingUnavailable:
                        return "Billing seems to be unavailable, please try again later.";
                    case PurchaseError.PaymentInvalid:
                        return "Payment seems to be invalid, please try again.";
                    case PurchaseError.PaymentNotAllowed:
                        return "Payment does not seem to be enabled/allowed, please try again.";
                    case PurchaseError.UserCancelled:
                        return "Cancelled";
                    default:
                        Log.For(this).Error(ex);
                        return GeneralError;
                }
            }
            catch (Exception ex)
            {
                Log.For(this).Error(ex);
                return GeneralError;
            }
        }

        static async Task KeepTrying()
        {
            if (StartedTrying) return;
            else StartedTrying = true;

            for (var attempts = 10; attempts > 0; attempts--)
            {
                if (await BillingContext<T>.Current.RestoreSubscription()) return;
                await Task.Delay(2.Seconds());
            }
        }
    }
}