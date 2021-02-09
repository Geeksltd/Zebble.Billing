namespace Zebble.Billing
{
    public static partial class BillingContext
    {
        public static bool IsSubscribed() => IsStarted() && !IsExpired() && !IsCanceled();

        public static bool IsStarted() => Subscription.IsStarted();

        public static bool IsExpired() => Subscription.IsExpired();

        public static bool IsCanceled() => Subscription.IsCanceled();
    }
}
