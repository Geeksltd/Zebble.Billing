namespace Zebble.Billing
{
    using Plugin.InAppBilling;
    using System;

    static class EnumExtensions
    {
        public static ItemType GetItemType(this Product @this)
            => @this.Type.GetItemType();

        public static ItemType GetItemType(this ProductType @this)
        {
            return @this switch
            {
                ProductType.Subscription => ItemType.Subscription,
                ProductType.InAppPurchase => ItemType.InAppPurchase,
                _ => throw new ArgumentOutOfRangeException(nameof(@this)),
            };
        }
    }
}