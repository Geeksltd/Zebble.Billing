namespace Zebble.Billing
{
    public enum SubscriptionStatus { None, Subscribed, Expired, Canceled }

    public enum ProductType { Subscription, InAppPurchase }

    public enum PurchaseVerificationStatus { Failed, UserMismatched, Verified }
}
