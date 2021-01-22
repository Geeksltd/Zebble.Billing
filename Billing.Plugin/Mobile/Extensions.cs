namespace Zebble.Billing
{
    using Olive;
#if CAFEBAZAAR
    using Plugin.InAppBilling.Abstractions;
#else
    using Plugin.InAppBilling;
#endif
    using System;
    using System.Linq;

    public static class Extensions
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

        public static PurchaseRecognizedEventArgs ToEventArgs(this InAppBillingPurchase purchase)
        {
            return new PurchaseRecognizedEventArgs
            {
                Id = purchase.Id,
                ProductId = purchase.ProductId,
                TransactionDateUtc = purchase.TransactionDateUtc,
                PurchaseToken = purchase.PurchaseToken
            };
        }

        public static Product GetProduct(this InAppBillingProduct @this)
        {
            return @this.ProductId.GetProduct();
        }

        public static Product GetProduct(this InAppBillingPurchase @this)
        {
            return @this.ProductId.GetProduct();
        }

        public static Product GetProduct(this string @this)
        {
            return ProductsCache.RegisteredProducts.FirstOrDefault(x => x.Id == @this);
        }

        public static DateTime GetExpiryUtc(this InAppBillingPurchase @this)
        {
            var product = @this.GetProduct();

            if (product?.IsLifetime == false)
                return @this.TransactionDateUtc.AddYears(1).AddDays(1);

            if (product?.IsLifetime == true)
                return @this.TransactionDateUtc.AddYears(100).AddDays(1);

            return @this.TransactionDateUtc.AddMonths(3);
        }

        public static bool IsAnyOf<T>(this T @this, params T[] options)
        {
            return options.Contains(@this);
        }
    }
}