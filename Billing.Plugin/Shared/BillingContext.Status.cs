namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;

    partial class BillingContext
    {
        public bool IsSubscribed => IsStarted && !IsExpired;

        public Product CurrentProduct => GetProduct(Subscription?.ProductId);

        public DateTime? SubscriptionDate => Subscription?.SubscriptionDate;

        public DateTime? ExpirationDate => Subscription?.ExpirationDate;

        public DateTime? CancellationDate => Subscription?.CancellationDate;

        public bool IsStarted => Subscription?.IsStarted() ?? false;

        public bool IsExpired => Subscription?.IsExpired() ?? false;

        public bool IsCanceled => Subscription?.IsCanceled() ?? false;
    }
}
