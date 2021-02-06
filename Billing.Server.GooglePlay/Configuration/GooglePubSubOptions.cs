namespace Zebble.Billing
{
    using System;
    using Olive;

    public class GooglePubSubOptions : GoogleServicesOptionsBase
    {
        public string SubscriptionId { get; set; }

        internal new bool Validate()
        {
            if (SubscriptionId.IsEmpty()) throw new ArgumentNullException(nameof(SubscriptionId));

            return base.Validate();
        }
    }
}
