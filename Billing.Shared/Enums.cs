namespace Zebble.Billing
{
    public enum ProductType { Subscription, InAppPurchase, Voucher }

    public enum PurchaseAttemptStatus { Failed, UserMismatchedAndBlocked, UserMismatchedAndReplaced, Succeeded }

    public enum VoucherApplyStatus { InvalidCode, Expired, Succeeded }
}
