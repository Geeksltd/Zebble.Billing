namespace Zebble.Billing
{
    using System;

    public class SubscriptionPurchasedEventArgs<T> : EventArgs where T : Product
    {
        public string Id { get; set; }
        public T Product { get; set; }
        public DateTime TransactionDateUtc { get; set; }
        public string PurchaseToken { get; set; }
    }
}
