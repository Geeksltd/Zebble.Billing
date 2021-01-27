namespace Zebble.Billing
{
    public enum SubscriptionPlatform { AppStore, GooglePlay, CafeBazaar, Voucher }

    public enum SubscriptionType { Standard, Pro }

    public enum SubscriptionStatus { None, Subscribed, Expired, FreeStarted, FreeExpiring, FreeExpired }

    public enum ProductType { Subscription, InAppPurchase }
}
