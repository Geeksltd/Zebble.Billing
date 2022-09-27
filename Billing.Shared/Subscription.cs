namespace Zebble.Billing
{
    using System;

    public partial class Subscription
    {
        public string ProductId { get; set; }

        public DateTime? SubscriptionDate { get; set; }
        public DateTime? SubscriptionDateOnly => SubscriptionDate?.Date;
        public DateTime? ExpirationDate { get; set; }
        public DateTime? CancellationDate { get; set; }
        public DateTime? CancellationDateOnly => CancellationDate?.Date;
    }
}
