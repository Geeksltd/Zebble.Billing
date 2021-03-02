namespace Zebble.Billing
{
    public class BillingOptions
    {
        public string VerifyPurchasePath { get; set; } = "app/verify-purchase";
        public string PurchaseAttemptPath { get; set; } = "app/purchase-attempt";
        public string SubscriptionStatusPath { get; set; } = "app/subscription-status";
    }
}
