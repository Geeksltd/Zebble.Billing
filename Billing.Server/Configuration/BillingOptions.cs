namespace Zebble.Billing
{
    public class BillingOptions
    {
        public string PurchaseAttemptPath { get; set; } = "app/purchase-attempt";
        public string SubscriptionStatusPath { get; set; } = "app/subscription-status";
    }
}
