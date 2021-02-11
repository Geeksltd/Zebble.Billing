namespace Zebble.Billing
{
    using System;

    public class SubscriptionRestoredEventArgs<T> : EventArgs where T : Product
    {
        public T Product { get; set; }
        public Subscription Subscription { get; set; }
    }
}
