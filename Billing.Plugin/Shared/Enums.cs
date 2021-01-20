namespace Zebble.Billing
{
    public enum SubscriptionPlatform { AppStore, PlayStore, CafeBazaar, Voucher }

    public enum SubscriptionType { Standard, Pro }

    public enum SubscriptionStatus { None, Subscribed, Expired, FreeStarted, FreeExpiring, FreeExpired }
}
