namespace Zebble.Billing
{
    using Olive;

    public static partial class EntityExtensions
    {
        internal static SubscriptionStatus GetStatus(this Subscription @this)
        {
            if (@this.IsCanceled()) return SubscriptionStatus.Canceled;

            if (@this.IsExpired()) return SubscriptionStatus.Expired;

            if (@this.IsStarted()) return SubscriptionStatus.Subscribed;

            return SubscriptionStatus.None;
        }

        public static bool IsStarted(this Subscription @this) => @this?.SubscriptionDate?.IsInThePast() ?? false;

        public static bool IsExpired(this Subscription @this) => @this?.ExpirationDate?.IsInThePast() ?? false;

        public static bool IsCanceled(this Subscription @this) => @this?.CancellationDate?.IsInThePast() ?? false;
    }
}
