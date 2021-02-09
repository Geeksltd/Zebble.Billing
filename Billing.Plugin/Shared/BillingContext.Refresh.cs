namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Zebble;
    using Olive;

    partial class BillingContext
    {
        public static async Task Refresh()
        {
            try { await DoRefresh(); }
            catch (Exception ex) { Log.For<Subscription>().Error(ex); }
        }

        public static async Task BackgroundRefresh()
        {
            while (User == null) await Task.Delay(500);
            if (!IsSubscribed()) return;

            await UIContext.AwaitConnection();
            try { await DoRefresh(); }
            catch { /*Ignore*/ }
        }

        static async Task DoRefresh()
        {
            if (User == null) return;

            var url = new Uri(Options.BaseUri, Options.SubscriptionStatusPath).ToString();
            var current = await BaseApi.Post<Subscription>(url, new { User.Ticket, User.UserId }, errorAction: OnError.Ignore);
            if (current == null) return;

            if (Subscription == current)
                return;

            Subscription = current;

            await SubscriptionRestored.Raise(current.ToEventArgs());
        }
    }
}
