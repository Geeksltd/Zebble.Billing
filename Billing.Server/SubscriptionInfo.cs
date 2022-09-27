namespace Zebble.Billing
{
    using System;

    public class SubscriptionInfo
    {
        public SubscriptionQueryStatus Status { get; private set; }

        public string UserId { get; set; }
        public string TransactionId { get; set; }
        public DateTime? SubscriptionDate { get; set; }
        public DateTime? SubscriptionDateOnly => SubscriptionDate?.Date;
        public DateTime? ExpirationDate { get; set; }
        public DateTime? CancellationDate { get; set; }
        public DateTime? CancellationDateOnly => CancellationDate?.Date;
        public bool AutoRenews { get; set; }

        public SubscriptionInfo(SubscriptionQueryStatus status = SubscriptionQueryStatus.Succeeded) => Status = status;

        public static SubscriptionInfo NotFound = new(SubscriptionQueryStatus.NotFound);
    }
}
