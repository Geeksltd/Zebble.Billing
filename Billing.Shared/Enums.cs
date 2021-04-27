namespace Zebble.Billing
{
    public enum SubscriptionStatus { None, Subscribed, Expired, Canceled }

    public enum PurchaseVerificationStatus { Failed, UserMismatched, Verified }

    public enum VoucherApplyStatus { InvalidCode, Expired, Succeeded }
}
