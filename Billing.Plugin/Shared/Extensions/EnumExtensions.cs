namespace Zebble.Billing
{
    using Plugin.InAppBilling;
    using System;

    static class EnumExtensions
    {
        public static ItemType GetItemType(this Product @this)
        {
            return @this.Type switch
            {
                ProductType.Subscription => ItemType.Subscription,
                ProductType.InAppPurchase => ItemType.InAppPurchase,
                ProductType.Voucher => ItemType.Voucher,
                _ => throw new ArgumentOutOfRangeException(nameof(@this.Type)),
            };
        }
    }
}