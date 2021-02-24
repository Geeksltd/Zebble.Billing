namespace Zebble.Billing
{
    using System.Threading.Tasks;
    using Plugin.InAppBilling;

    class PurchaseVerificator : IInAppBillingVerifyPurchase
    {
        public Task<bool> VerifyPurchase(string signedData, string signature, string productId = null, string transactionId = null)
        {
            return BillingContext.Current.VerifyPurchase(new VerifyPurchaseEventArgs
            {
                ProductId = productId,
                TransactionId = transactionId,
                ReceiptData = signedData
            });
        }
    }
}