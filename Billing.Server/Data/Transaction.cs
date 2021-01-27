namespace Zebble.Billing
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Transactions")]
    public class Transaction
    {
        public Guid TransactionId { get; set; }
        public Guid SubscriptionId { get; set; }

        public SubscriptionPlatform Platform { get; set; }

        public DateTime Date { get; set; }

        public string Details { get; set; }
    }
}
