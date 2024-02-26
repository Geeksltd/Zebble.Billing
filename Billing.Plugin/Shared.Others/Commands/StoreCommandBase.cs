namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Olive;
    using Plugin.InAppBilling;

    abstract class StoreCommandBase<TOut>
    {
        protected static IInAppBilling Billing => CrossInAppBilling.Current;

        protected abstract Task<TOut> DoExecute(IBillingUser user);

        public Task<TOut> Execute(IBillingUser user) => TryExecute(user);

        async Task<TOut> TryExecute(IBillingUser user)
        {
            try
            {
                if (user is null) throw new ArgumentNullException(nameof(user));

                if (!CrossInAppBilling.IsSupported) return default;

                var connected = await Billing.ConnectAsync().ConfigureAwait(false);
                if (connected == false) return default;

                return await DoExecute(user).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (await UIContext.IsOnline().ConfigureAwait(false))
                    Log.For(this).Error(ex);
                throw;
            }
            finally
            {
                try
                {
                    if (CrossInAppBilling.IsSupported) await Billing.DisconnectAsync().ConfigureAwait(false);
                }
                catch (Exception ex) { Log.For(this).Error(ex, "Failed to disconnect from billing!"); }
            }
        }
    }
}