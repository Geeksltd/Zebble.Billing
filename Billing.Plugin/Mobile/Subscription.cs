namespace Zebble.Billing
{
    using Olive;
    using System;

    public partial class Subscription
    {
        // TODO: remove
        [Obsolete]
        public string System
        {
            get { return Platform.ToString(); }
            set { Platform = value.To<SubscriptionPlatform>(); }
        }

        // TODO: remove
        [Obsolete]
        public DateTime PurchasedUtc
        {
            get { return DateSubscribed; }
            set { DateSubscribed = value; }
        }

        // TODO: remove
        [Obsolete]
        public DateTime ExpiryUtc
        {
            get { return ExpiryDate; }
            set { ExpiryDate = value; }
        }
    }
}
