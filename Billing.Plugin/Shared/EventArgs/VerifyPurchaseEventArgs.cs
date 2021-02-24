namespace Zebble.Billing
{
    using System;

    public class VerifyPurchaseEventArgs : EventArgs
    {
        public string ProductId { get; set; }
        public string TransactionId { get; set; }
        public string ReceiptData { get; set; }
    }
}
