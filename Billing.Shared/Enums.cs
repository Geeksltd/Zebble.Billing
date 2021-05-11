namespace Zebble.Billing
{
    public enum ProductType { Subscription, InAppPurchase, Voucher }

    public enum PurchaseVerificationStatus { Failed, UserMismatched, Verified }

    public enum VoucherApplyStatus { InvalidCode, Expired, Succeeded }
}
