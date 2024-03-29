﻿namespace Zebble.Billing
{
    using System;
    using Olive;

    public static partial class EntityExtensions
    {
        public static bool IsStarted(this Subscription @this) => IsInThePast(@this?.SubscriptionDate);

        public static bool IsExpired(this Subscription @this) => IsInThePast(@this?.ExpirationDate);

        public static bool IsCanceled(this Subscription @this) => IsInThePast(@this?.CancellationDate);

        static bool IsInThePast(DateTime? @this)
        {
            if (@this is null) return false;
            if (@this.Value.Kind == DateTimeKind.Local) return @this.Value.IsInThePast();
            return @this.Value.ToLocal().IsInThePast();
        }
    }
}
