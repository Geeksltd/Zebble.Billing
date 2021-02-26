namespace Zebble.Billing
{
    using System;
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

        public static bool IsStarted(this Subscription @this) => IsInThePast(@this?.SubscriptionDate);

        public static bool IsExpired(this Subscription @this) => IsInThePast(@this?.ExpirationDate);

        public static bool IsCanceled(this Subscription @this) => IsInThePast(@this?.CancellationDate);

        static bool IsInThePast(DateTime? @this) => @this?.ToLocal().IsInThePast() ?? false;
    }
}
