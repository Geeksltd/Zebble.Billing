﻿namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Zebble;
    using Olive;

    partial class BillingContext
    {
        public async Task BackgroundRefresh()
        {
            while (User == null) await Task.Delay(500);
            if (!IsSubscribed) return;

            await UIContext.AwaitConnection();
            try { await DoRefresh(); }
            catch { /*Ignore*/ }
        }

        public async Task Refresh()
        {
            try { await DoRefresh(); }
            catch (Exception ex) { Log.For<Subscription>().Error(ex); }
        }

        async Task DoRefresh()
        {
            if (User == null) return;

            var url = new Uri(Options.BaseUri, Options.SubscriptionStatusPath).ToString();
            var current = await BaseApi.Post<Subscription>(url, new { User.Ticket, User.UserId }, errorAction: OnError.Ignore);

            Subscription = current;

            await SubscriptionRestored.Raise(current.ToEventArgs());
        }
    }
}
