namespace Zebble.Billing
{
    using System;
    using Apple.Receipt.Verificator.Models;

    static class EnumExtensions
    {
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
