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
            set
            {
                if (value is not null)
                    value = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                subscriptionDate = value;
                SubscriptionDateOnly = value?.Date;
            }
        }

        public DateTime? SubscriptionDateOnly { get; set; }

        DateTime? expirationDate;
        public DateTime? ExpirationDate
        {
            get => expirationDate;
            set {
                if (value is not null)
                    value = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                expirationDate = value;
                ExpirationDateOnly = value?.Date;
            }
        }

        public DateTime? ExpirationDateOnly { get; set; }

        DateTime? cancellationDate;
        public DateTime? CancellationDate
        {
            get => cancellationDate;
            set {
                if (value is not null)
                    value = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                cancellationDate = value;
                CancellationDateOnly = value?.Date;
            }
        }

        public DateTime? CancellationDateOnly { get; set; }
    }
}
