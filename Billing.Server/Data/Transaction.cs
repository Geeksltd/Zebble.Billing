namespace Zebble.Billing
{
    using System;

    public class Transaction
    {
        public virtual string Id { get; set; }
        public virtual string SubscriptionId { get; set; }

        public string Platform { get; set; }

        public DateTime Date { get; set; }

        public string Details { get; set; }
    }
}
