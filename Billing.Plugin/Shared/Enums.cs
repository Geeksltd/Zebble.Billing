namespace Zebble.Billing
{
    using System;

    public enum SubscriptionPlatform { AppStore, [Obsolete] GooglePlay, PlayStor, CafeBazaar, Voucher }

    public enum SubscriptionType { Standard, Pro }

    public enum SubscriptionStatus { None, Subscribed, Expired, FreeStarted, FreeExpiring, FreeExpired }

    public enum ProductType { Subscription, InAppPurchase }
}
