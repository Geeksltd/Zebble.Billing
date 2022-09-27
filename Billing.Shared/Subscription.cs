namespace Zebble.Billing
{
    using System;

    public partial class Subscription
    {
        public string ProductId { get; set; }

        DateTime? subscriptionDate;
        public DateTime? SubscriptionDate
        {
            get => subscriptionDate;
            set { subscriptionDate = value; SubscriptionDateOnly = value?.Date; }
        }
        public DateTime? SubscriptionDateOnly { get; set; }

        DateTime? expirationDate;
        public DateTime? ExpirationDate
        {
            get => expirationDate;
            set { expirationDate = value; ExpirationDateOnly = value?.Date; }
        }
        public DateTime? ExpirationDateOnly { get; set; }

        DateTime? cancellationDate;
        public DateTime? CancellationDate
        {
            get => cancellationDate;
            set { cancellationDate = value; CancellationDateOnly = value?.Date; }
        }
        public DateTime? CancellationDateOnly { get; set; }
    }
}
