namespace Zebble.Billing
{
    using System.Threading.Tasks;
    using Plugin.InAppBilling;

    class PurchaseVerificator : IInAppBillingVerifyPurchase
    {
        public PurchaseVerificationStatus Status { get; set; }

        public async Task<bool> VerifyPurchase(string signedData, string signature, string productId = null, string transactionId = null)
        {
            var result = await BillingContext.Current.VerifyPurchase(new VerifyPurchaseEventArgs
            {
                ProductId = productId,
                TransactionId = transactionId,
                ReceiptData = signedData
            });

            Status = result.Status;

            return Status == PurchaseVerificationStatus.Verified;
        }
    }
}