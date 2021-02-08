namespace Zebble.Billing
{
    using Olive;
    using System;
    using System.Linq;

    static class LookupExtensions
    {
        public static bool IsAnyOf<T>(this T @this, params T[] options) => options.Contains(@this);
    }
}