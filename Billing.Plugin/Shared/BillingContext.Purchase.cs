namespace Zebble.Billing
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Olive;

    partial class BillingContext
    {
        /// <summary>
        /// Triggers a new purchase process.
        /// </summary>
        public async Task<(PurchaseResult, string)> PurchaseSubscription(IBillingUser user, string productId)
        {
#if MVVM || UWP
            return (PurchaseResult.AppStoreUnavailable, null);
#else
            var product = GetProduct(productId) ?? throw new Exception($"Product with id '{productId}' not found.");
            return await new PurchaseSubscriptionCommand(product).Execute(user).ConfigureAwait(false);
#endif
        }

        /// <summary>
        /// Restores all already purchased subscriptions.
        /// </summary>
        /// <remarks>If you pass the true for `userRequest`, and no active subscription is found, it will throw an exception.</remarks>
        public async Task<bool> RestoreSubscription(IBillingUser user, bool userRequest = false)
        {
            var errorMessage = "";

#if !MVVM && !UWP
            try { await new RestoreSubscriptionCommand().Execute(user).ConfigureAwait(false); }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                Log.For(typeof(BillingContext)).Error(ex);
            }
#endif

            var successful = false;
            try
            {
                await Refresh(user).ConfigureAwait(false);
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

        internal async Task<PurchaseAttemptResult> PurchaseAttempt(IBillingUser user, SubscriptionPurchasedEventArgs args)
        {
            if (user is null) throw new Exception("User is not available.");

            if (await UIContext.IsOffline().ConfigureAwait(false)) throw new Exception("Network connection is not available.");

            var url = new Uri(Options.BaseUri, Options.PurchaseAttemptPath).ToString();
            var @params = new { user.Ticket, user.UserId, Platform = PaymentAuthority, args.ProductId, args.SubscriptionId, args.TransactionId, args.PurchaseToken, args.ReplaceConfirmed };

            var result = await BaseApi.Post<PurchaseAttemptResult>(url, @params, OnError.Ignore, showWaiting: false).ConfigureAwait(false);

            if (result?.Status == PurchaseAttemptStatus.Succeeded)
                await SubscriptionPurchased.Raise(args).ConfigureAwait(false);

            return result;
        }

        async Task<(PurchaseResult, string)> ProcessPurchase(IBillingUser user, SubscriptionPurchasedEventArgs args)
        {
            var replaceConfirmed = false;

            while (true)
            {
                args.ReplaceConfirmed = replaceConfirmed;
                var result = await PurchaseAttempt(user, args).ConfigureAwait(false);
                if (result is null) return (PurchaseResult.Unknown, null);

                if (result.Status == PurchaseAttemptStatus.Succeeded)
                    return (PurchaseResult.Succeeded, replaceConfirmed ? result.OriginUserId : null);

                if (result.Status != PurchaseAttemptStatus.UserMismatched) break;

                // In iOS, we get PurchaseAttemptStatus.UserMismatched error when we try to purchase an item
                // that is already associated with another app-specific account

                // BillingOptions.UserMismatchResolvingStrategy is set to block
                if (result.NewUserId.IsEmpty())
                    return (PurchaseResult.UserMismatched, null);

#if !NETCOREAPP
                var promptResult = await Dialogs.Current.Decide(
                    "Warning",
                    $"This account subscription was previously linked to {result.OriginUserId}. Where do you want your subscription?", new[] {
                    new KeyValuePair<string, string>("Previous account", result.OriginUserId),
                    new KeyValuePair<string, string>("This account", result.NewUserId),
                }).ConfigureAwait(false);

                if (promptResult == result.NewUserId)
                {
                    replaceConfirmed = true;
                    continue;
                }
#endif

                return (PurchaseResult.UserMismatched, null);
            }

            return (PurchaseResult.NotCompleted, null);
        }
    }
}
