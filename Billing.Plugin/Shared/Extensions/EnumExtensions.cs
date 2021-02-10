namespace Zebble.Billing
{
    using Plugin.InAppBilling;
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