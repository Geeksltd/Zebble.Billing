namespace Zebble.Billing
{
#if CAFEBAZAAR
    using Plugin.InAppBilling.Abstractions;
#else
    using Plugin.InAppBilling;
#endif
    using System;

    static class EnumExtensions
    {
        public static ItemType ToItemType(this ProductType type)
        {
            return type switch
            {
                ProductType.Subscription => ItemType.Subscription,
                ProductType.InAppPurchase => ItemType.InAppPurchase,
                _ => throw new ArgumentOutOfRangeException(nameof(type)),
            };
        }
    }
}