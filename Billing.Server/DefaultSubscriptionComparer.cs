namespace Zebble.Billing
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public class DefaultSubscriptionComparer : ISubscriptionComparer
    {
        public virtual int Compare([AllowNull] Subscription @this, [AllowNull] Subscription that)
        {
            _ = @this ?? throw new ArgumentNullException(nameof(@this));
            _ = that ?? throw new ArgumentNullException(nameof(that));

            if (ReferenceEquals(@this, that)) return 0;

            var isStarted = CheckCriteria(@this, that, x => x.IsStarted());
            if (isStarted.HasValue) return isStarted.Value;

            var isExpired = CheckCriteria(@this, that, x => !x.IsExpired());
            if (isExpired.HasValue) return isExpired.Value;

            var isCanceled = CheckCriteria(@this, that, x => !x.IsCanceled());
            return isCanceled ?? 0;
        }

        int? CheckCriteria(Subscription @this, Subscription that, Func<Subscription, bool> valueAccessor)
        {
            var thisValue = valueAccessor(@this);
            var thatValue = valueAccessor(that);

            if (!thisValue && !thatValue) return 0;
            if (!thisValue) return -1;
            if (!thatValue) return 1;
            return null;
        }
    }
}