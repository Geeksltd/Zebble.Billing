namespace Zebble.Billing
{
    using Olive;
    using System;

    public class Subscription
    {
        public virtual string Id { get; set; }

        public virtual string UserId { get; set; }

        public string Platform { get; set; }

        public virtual string SubscriptionId { get; set; }
        public virtual string TransactionId { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string PurchaseToken { get; set; }

        public DateTime? LastUpdate { get; set; }

        public bool? AutoRenews { get; set; }

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
            set
            {
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
            set
            {
                if (value is not null)
                    value = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                cancellationDate = value;
                CancellationDateOnly = value?.Date;
            }
        }

        public DateTime? CancellationDateOnly { get; set; }

        public bool IsStarted() => IsInThePast(SubscriptionDate);
        public bool IsExpired() => IsInThePast(ExpirationDate);
        public bool IsCanceled() => IsInThePast(CancellationDate);

        static bool IsInThePast(DateTime? @this)
        {
            if (@this is null) return false;
            if (@this.Value.Kind == DateTimeKind.Local) return @this.Value.IsInThePast();
            return @this.Value.ToLocal().IsInThePast();
        }
    }
}
