namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Olive;
    using Plugin.InAppBilling;

    abstract class SubscriptionCommand<TOut>
    {
        protected static IInAppBilling Billing => CrossInAppBilling.Current;

        protected abstract Task<TOut> DoExecute();

        public Task<TOut> Execute() => TryExecute();

        async Task<TOut> TryExecute()
        {
            try
            {
                if (!CrossInAppBilling.IsSupported) return default;

                var connected = await Billing.ConnectAsync();
                if (!connected) return default;

                return await DoExecute();
            }
            catch (Exception ex)
            {
                if (await UIContext.IsOnline())
                    Log.For(this).Error(ex);
                throw;
            }
            finally
            {
                try { await Billing.DisconnectAsync(); }
                catch (Exception ex) { Log.For(this).Error(ex, "Failed to disconnect from billing!"); }
            }
        }
    }
}