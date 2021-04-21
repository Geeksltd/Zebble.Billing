namespace Zebble.Billing
{
    using Olive;
    using System;
    using System.Linq;
    using System.Reflection;

    public static class ReflectionExtensions
    {
        public static void CopyPropertiesFrom<TProxy, T>(this TProxy @this, T that) where TProxy : IBillingDynamoDbProxy, T
        {
            @this.CopyPropertiesFrom<T>(that);
        }

        internal static void CopyPropertiesFrom<T>(this T @this, T that, params string[] excludedProps)
        {
            typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                           .Where(x => x.CanRead && x.CanWrite)
                           .Where(x => !excludedProps.Contains(x.Name))
                           .Do(p => p.SetValue(@this, p.GetValue(that)));
        }
    }
}
