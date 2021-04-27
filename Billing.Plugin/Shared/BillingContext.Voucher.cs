namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Zebble;

    partial class BillingContext
    {
        internal async Task ApplyVoucher(VoucherApplyEventArgs args)
        {
            if (await UIContext.IsOffline()) throw new Exception("Network connection is not available.");

            if (User == null) throw new Exception("User is not available.");

            var url = new Uri(Options.BaseUri, Options.VoucherApplyPath).ToString();
            var @params = new { User.Ticket, User.UserId, args.Code };

            await BaseApi.Post(url, @params, OnError.Ignore, showWaiting: false);

            await Refresh();
        }
    }
}
