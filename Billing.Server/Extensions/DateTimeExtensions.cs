namespace Zebble.Billing
{
    using System;

    public static class DateTimeExtensions
    {
        public static DateTime? ToDateTime(this long? @this)
        {
            if (@this is null) return null;

            return @this.Value.ToDateTime();
        }

        public static DateTime ToDateTime(this long @this)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(@this).DateTime;
        }
    }
}
