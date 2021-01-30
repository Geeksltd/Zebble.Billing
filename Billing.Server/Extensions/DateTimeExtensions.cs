namespace Zebble.Billing
{
    using System;

    static class DateTimeExtensions
    {
        public static DateTime? ToDateTime(this long? @this)
        {
            if (@this == null) return null;

            return @this.Value.ToDateTime();
        }

        public static DateTime ToDateTime(this long @this)
        {
            return DateTime.UnixEpoch.AddMilliseconds(@this);
        }
    }
}
