namespace Zebble.Billing
{
    using System;

    public class HuaweiValidatePurchaseResult
    {
        public DateTime? SubscriptionDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public DateTime? CancellationDate { get; set; }

        public bool AutoRenews { get; set; }
    }
}
