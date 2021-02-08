namespace Zebble.Billing
{
    using System;

    public partial class Subscription
    {
        public string ProductId { get; set; }

        public DateTime? SubscriptionDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? CancellationDate { get; set; }
    }
}
