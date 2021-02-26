namespace Zebble.Billing
{
    using System;
    using System.Linq;

    public static class EnumExtensions
    {
        public static bool IsAnyOf<T>(this T @this, params T[] others) => others.Contains(@this);
    }
}
