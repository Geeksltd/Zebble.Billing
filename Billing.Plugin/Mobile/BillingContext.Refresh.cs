namespace Zebble.Billing
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Zebble;
    using Olive;

    partial class BillingContext
    {
        public static AsyncEvent<SubscriptionRestoredEventArgs> Restored = new();

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
            var user = User;
            if (user == null) return;

            var args = new { user.Ticket, user.UserId, Tokens = user.SubscriptionTokens };
            var result = await BaseApi.Post<Subscription[]>(BaseUrl + "refresh", args, errorAction: OnError.Ignore);
            var current = result?.FirstOrDefault();
            if (current == null) return;

            var product = current.ProductId.GetProduct();

            if (user.SubscriptionExpiry != current.ExpiryDate || user.SubscriptionType != product.SubscriptionType)
            {
                await Restored.Raise(new SubscriptionRestoredEventArgs
                {
                    Product = product,
                    SubscriptionExpiry = current.ExpiryDate
                });
            }
        }
    }
}
