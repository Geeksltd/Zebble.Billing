﻿namespace Zebble.Billing
{
    using Olive;

    public static class EntityExtensions
    {
        public static bool IsExpired(this Subscription @this) => @this.ExpiryDate.IsInThePast();

        public static bool IsCanceled(this Subscription @this) => @this.CancellationDate?.IsInThePast() ?? true;
    }
}
