namespace Zebble.Billing
{
    using System;
    using System.Linq;
    using Apple.Receipt.Verificator.Models;

    static class EnumExtensions
    {
        public static bool IsAnyOf<T>(this T @this, params T[] others) => others.Contains(@this);

        public static AppleReceiptVerificationType ToVerificationType(this AppStoreEnvironment environment)
        {
            return environment switch
            {
                AppStoreEnvironment.Sandbox => AppleReceiptVerificationType.Sandbox,
                AppStoreEnvironment.Production => AppleReceiptVerificationType.Production,
                _ => throw new ArgumentOutOfRangeException(nameof(environment)),
            };
        }
    }
}
