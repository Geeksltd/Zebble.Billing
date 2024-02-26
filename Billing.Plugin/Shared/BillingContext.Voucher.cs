namespace Zebble.Billing
{
    using Olive;
    using System;
    using System.Threading.Tasks;
    using Zebble;

    partial class BillingContext
    {
        public async Task<VoucherApplyStatus?> ApplyVoucher(IBillingUser user, string code)
        {
            if (user is null) throw new ArgumentNullException(nameof(user));

            if (code.IsEmpty()) throw new ArgumentNullException(nameof(code));

            if (await UIContext.IsOffline().ConfigureAwait(false)) throw new Exception("Network connection is not available.");

            var url = new Uri(Options.BaseUri, Options.VoucherApplyPath).ToString();
            var @params = new { user.Ticket, user.UserId, Code = code };

            var result = await BaseApi.Post<ApplyVoucherResult>(
                url, @params, OnError.Ignore, showWaiting: false).ConfigureAwait(false);

            if (result?.Status == VoucherApplyStatus.Succeeded) await Refresh(user).ConfigureAwait(false);

            return result?.Status;
        }
    }
}
