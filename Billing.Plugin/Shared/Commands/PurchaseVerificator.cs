namespace Zebble.Billing
{
    using System.Threading.Tasks;
    using Plugin.InAppBilling;

    class PurchaseVerificator : IInAppBillingVerifyPurchase
    {
        public PurchaseVerificationResult Status { get; set; }

        public async Task<bool> VerifyPurchase(string signedData, string signature, string productId = null, string transactionId = null)
        {
            Status = await BillingContext.Current.VerifyPurchase(new VerifyPurchaseEventArgs
            {
                ProductId = productId,
                TransactionId = transactionId,
                ReceiptData = signedData
            });

            return Status == PurchaseVerificationResult.Verified;
        }
    }
}