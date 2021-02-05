namespace Zebble.Billing
{
    using System;

    public class Transaction
    {
        public string TransactionId { get; set; }
        public string SubscriptionId { get; set; }

        public string Platform { get; set; }

        public DateTime Date { get; set; }

        public string Details { get; set; }
    }
}
