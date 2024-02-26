namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Huawei.Hmf.Extensions;
    using Huawei.Hms.Iap;
    using Huawei.Hms.Iap.Entity;
    using Olive;

    abstract class StoreCommandBase<TOut>
    {
        protected static IIapClient Billing = Iap.GetIapClient(UIRuntime.CurrentActivity);

        protected abstract Task<TOut> DoExecute(IBillingUser user);

        public Task<TOut> Execute(IBillingUser user) => TryExecute(user);

        async Task<TOut> TryExecute(IBillingUser user)
        {
            try
            {
                if (user is null) throw new ArgumentNullException(nameof(user));

                BillingContext.ConfigureServices(UIRuntime.CurrentActivity);

                var result = await Billing.IsEnvReady()
                    .AsAsync<IsEnvReadyResult>()
                    .ConfigureAwait(false);

                if (!result.Status.IsSuccess) return default;

                return await DoExecute(user).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (await UIContext.IsOnline().ConfigureAwait(false))
                    Log.For(this).Error(ex);
                throw;
            }
        }
    }
}