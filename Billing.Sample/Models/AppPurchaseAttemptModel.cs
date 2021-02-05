namespace Zebble.Billing.Sample
{
    public class AppPurchaseAttemptModel
    {
        public string ProductId { get; set; }
        public string UserId { get; set; }
        public string Platform { get; set; }
        public string PurchaseToken { get; set; }
    }
}
