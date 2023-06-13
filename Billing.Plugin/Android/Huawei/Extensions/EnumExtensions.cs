namespace Zebble.Billing
{
    using System;

    static class EnumExtensions
    {
        public static int GetPriceType(this Product @this)
            => @this.Type.GetPriceType();

        public static int GetPriceType(this ProductType @this)
        {
            return @this switch
            {
                ProductType.Subscription => 2,
                ProductType.InAppPurchase => 1,
                _ => throw new ArgumentOutOfRangeException(nameof(@this)),
            };
        }
    }
}