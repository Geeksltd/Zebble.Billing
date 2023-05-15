namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Plugin.InAppBilling;
    using Olive;
    using System.Collections.Generic;

    class PurchaseSubscriptionCommand : StoreCommandBase<(PurchaseResult, string)>
    {
        readonly Product Product;

        public PurchaseSubscriptionCommand(Product product) => Product = product;

        protected override async Task<(PurchaseResult, string)> DoExecute(IBillingUser user)
        {
            var context = BillingContext.Current;

            try
            {
                await context.Refresh(user);

                if (context.IsSubscribed && (await context.CurrentProduct)?.Id == Product.Id) return (PurchaseResult.AlreadySubscribed, null);

#if CAFEBAZAAR && ANDROID
                var purchase = await Billing.PurchaseAsync(Product.Id, Product.GetItemType(), user.UserId);
#else
                var purchase = await Billing.PurchaseAsync(Product.Id, Product.GetItemType());
#endif
                var (result, originUserId) = await context.ProcessPurchase(user, purchase);
                if (result != PurchaseResult.Succeeded) return (result, null);

#if !(CAFEBAZAAR && ANDROID)
                if (purchase.State == PurchaseState.Purchased)
                    await Billing.FinalizePurchaseAsync(purchase.PurchaseToken);
#endif
                await context.Refresh(user);

                if (context.IsSubscribed) return (PurchaseResult.Succeeded, originUserId);

                if (purchase.State.IsAnyOf(PurchaseState.Restored, PurchaseState.Purchased, PurchaseState.Purchasing, PurchaseState.PaymentPending))
                    return (PurchaseResult.WillBeActivated, originUserId);

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