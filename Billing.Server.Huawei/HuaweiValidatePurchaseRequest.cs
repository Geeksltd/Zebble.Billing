namespace Zebble.Billing
{
    public class HuaweiValidatePurchaseRequest
    {
        public string PackageName { get; set; }
        public string PublicKey { get; set; }
        public string ProductId { get; set; }
        public string PurchaseToken { get; set; }
    }
}
