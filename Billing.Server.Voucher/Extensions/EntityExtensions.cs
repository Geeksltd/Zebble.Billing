namespace Zebble.Billing
{
    using System;
    using Olive;

    public static class EntityExtensions
    {
        public static DateTime? ExpirationDate(this Voucher @this) => @this.ActivationDate?.Add(@this.Duration);

        public static bool IsExpired(this Voucher @this) => @this.ExpirationDate()?.IsInThePast() == true;
    }
}
