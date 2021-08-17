namespace Zebble.Billing
{
    using System.Diagnostics.CodeAnalysis;

    class NeutralSubscriptionComparer : ISubscriptionComparer
    {
        public int Compare([AllowNull] Subscription @this, [AllowNull] Subscription that) => 0;
    }
}