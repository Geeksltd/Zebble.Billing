namespace Zebble.Billing
{
    using Huawei.Hms.Iap.Entity;
    using System;

    static partial class EventArgsExtensions
    {
        public static SubscriptionPurchasedEventArgs ToEventArgs(this InAppPurchaseData purchase)
        {
            return new SubscriptionPurchasedEventArgs
            {
                ProductId = purchase.ProductId,
                SubscriptionId = purchase.SubscriptionId,
                TransactionId = purchase.OrderID,
                TransactionDate = DateTimeOffset.FromUnixTimeMilliseconds(purchase.PurchaseTime).DateTime,
                PurchaseToken = purchase.PurchaseToken,
            };
        }
    }
}