namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Olive;

    partial class BillingContext
    {
        /// <summary>
        /// Triggers a new purchase process.
        /// </summary>
        public async Task<PurchaseResult> PurchaseSubscription(string productId)
        {
#if MVVM || UWP
            return PurchaseResult.AppStoreUnavailable;
#else
            var product = await GetProduct(productId) ?? throw new Exception($"Product with id '{productId}' not found.");
            return await new PurchaseSubscriptionCommand(product).Execute();
#endif
        }

        /// <summary>
        /// Restores all already purchased subscriptions.
        /// </summary>
        /// <remarks>If you pass the true for `userRequest`, and no active subscription is found, it will throw an exception.</remarks>
        public async Task<bool> RestoreSubscription(bool userRequest = false)
        {
            var errorMessage = "";

#if !MVVM && !UWP
            try { await new RestoreSubscriptionCommand().Execute(); }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                Log.For(typeof(BillingContext)).Error(ex);
            }
#endif

            var successful = false;
            try
            {
                await Refresh();
                successful = IsSubscribed;
            }
            catch (Exception ex)
            {
                if (errorMessage.IsEmpty()) errorMessage = ex.Message;
                Log.For(typeof(BillingContext)).Error(ex);
            }

            if (!successful && userRequest)
                throw new Exception(errorMessage.Or("Unable to find an active subscription."));

            return successful;
        }

        internal async Task<PurchaseAttemptResult> PurchaseAttempt(SubscriptionPurchasedEventArgs args)
        {
            if (await UIContext.IsOffline()) throw new Exception("Network connection is not available.");

            if (User == null) throw new Exception("User is not available.");

            var url = new Uri(Options.BaseUri, Options.PurchaseAttemptPath).ToString();
            var @params = new { User.Ticket, User.UserId, Platform = PaymentAuthority, args.ProductId, args.PurchaseToken };

            var result = await BaseApi.Post<PurchaseAttemptResult>(url, @params, OnError.Ignore, showWaiting: false);

            if (result?.Status == PurchaseAttemptStatus.Succeeded)
                await SubscriptionPurchased.Raise(args);

            return result;
        }
    }
}
