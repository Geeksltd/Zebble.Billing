namespace Zebble.Billing
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Subscription
    {
        public Guid SubscriptionId { get; set; }
    }
}
