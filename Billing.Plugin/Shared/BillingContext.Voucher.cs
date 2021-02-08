namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Zebble;
    using Olive;

    partial class BillingContext
    {
        public static async Task<bool> ApplyVoucher(string code)
        {
            if (await UIContext.IsOffline())
            {
                await Alert.Show("Network connection is not available.");
                return false;
            }

            try
            {
                var url = new Uri(Options.BaseUri, $"{Options.VoucherApplyPath}/{User.UserId}/{code}").ToString();
                var result = await BaseApi.Post<DateTime?>(url, null, OnError.Ignore, showWaiting: false);

                if (result == null) return false;

                if (result?.IsInTheFuture() == true)
                    await VoucherApplied.Raise(new VoucherAppliedEventArgs { VoucherCode = code });

                await RestoreSubscription(userRequest: true);

                return true;
            }
            catch { return false; }
        }
    }
}
