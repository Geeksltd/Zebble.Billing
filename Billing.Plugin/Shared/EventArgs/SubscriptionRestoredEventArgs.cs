namespace Zebble.Billing
{
    using System;

    public class SubscriptionRestoredEventArgs : EventArgs
    {
        public string ProductId { get; set; }
        public DateTime? SubscriptionDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? CancellationDate { get; set; }
    }
}
