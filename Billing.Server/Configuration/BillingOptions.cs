namespace Zebble.Billing
{
    public class BillingOptions
    {
        public string PurchaseAttemptPath { get; set; } = "app/purchase-attempt";
        public string PurchaseAttemptsPath { get; set; } = "app/purchase-attempts";
        public string SubscriptionStatusPath { get; set; } = "app/subscription-status";
        public UserMismatchResolvingStrategy UserMismatchResolvingStrategy { get; set; } = UserMismatchResolvingStrategy.Block;
    }
}
