namespace Zebble.Billing
{
    partial class BillingContext<T>
    {
        public bool IsSubscribed() => IsStarted() && !IsExpired() && !IsCanceled();

        public bool IsStarted() => Subscription.IsStarted();

        public bool IsExpired() => Subscription.IsExpired();

        public bool IsCanceled() => Subscription.IsCanceled();
    }
}
