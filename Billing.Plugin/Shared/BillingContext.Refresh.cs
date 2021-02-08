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
            if (!IsSubscribed(free: false) && !IsExpired()) return;

            await UIContext.AwaitConnection();
            try { await DoRefresh(); }
            catch { /*Ignore*/ }
        }

        static async Task DoRefresh()
        {
            if (User == null) return;

            var current = await BaseApi.Get<Subscription>($"{BaseUrl}subscription-status?ticket={User.Ticket}&userId={User.UserId}", errorAction: OnError.Ignore);
            if (current == null) return;

            if (Subscription == current)
                return;

            Subscription = current;

            await SubscriptionRestored.Raise(current.ToEventArgs());
        }
    }
}
