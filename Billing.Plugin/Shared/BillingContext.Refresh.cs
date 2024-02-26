namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Zebble;
    using Olive;

    partial class BillingContext
    {
        /// <summary>
        /// Queries the latest subscription status from the server in background.
        /// </summary>
        /// <remarks>An active internet connection is required.</remarks>
        public async Task BackgroundRefresh(IBillingUser user)
        {
            if (user is null) throw new ArgumentNullException(nameof(user));
            if ((user?.Ticket).IsEmpty()) return;

            await UIContext.AwaitConnection().ConfigureAwait(false);

            try { await DoRefresh(user).ConfigureAwait(false); }
            catch { /*Ignore*/ }
        }

        /// <summary>
        /// Queries the latest subscription status from the server.
        /// </summary>
        public async Task Refresh(IBillingUser user)
        {
            if (user is null) throw new ArgumentNullException(nameof(user));

            try { await DoRefresh(user).ConfigureAwait(false); }
            catch (Exception ex) { Log.For<Subscription>().Error(ex); }
        }

        async Task DoRefresh(IBillingUser user, bool retry = true)
        {
            var url = new Uri(Options.BaseUri, Options.SubscriptionStatusPath).ToString();
            var @params = new { user.Ticket, user.UserId };

            try
            {
                var current = await BaseApi.Post<Subscription>(
                    url, @params, errorAction: OnError.Throw, showWaiting: false).ConfigureAwait(false);

                if (HasChanged(Subscription, current) || SubscriptionFileStore.Exists(user) == false)
                {
                    Subscription = current;

                    await SubscriptionFileStore.Save(user).ConfigureAwait(false);

                    await SubscriptionRestored.Raise(current.ToEventArgs()).ConfigureAwait(false);
                }

                IsLoaded = true;
            }
            catch (Exception ex)
            {
                Log.For(this).Error(ex, $"Failed to refresh the billing data. {ex.Message}");
                if (retry) await DoRefresh(user, retry: false).ConfigureAwait(false);
            }
        }

        bool HasChanged(Subscription @this, Subscription that)
        {
            if (@this is null) return that is not null;
            if (that is null) return true;
            if (@this.ProductId != that.ProductId) return true;
            if (@this.SubscriptionDateOnly != that.SubscriptionDateOnly) return true;
            if (@this.ExpirationDateOnly != that.ExpirationDateOnly) return true;
            if (@this.CancellationDateOnly != that.CancellationDateOnly) return true;

            return false;
        }
    }
}
