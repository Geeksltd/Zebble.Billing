namespace Zebble.Billing
{
    using Olive;
    using System;

    class GooglePlayNotification
    {
        public DateTime EventTime { get; set; }
        public string PurchaseToken { get; set; }
        public string ProductId { get; set; }
        public string OriginalData { get; set; }
        public GooglePlaySubscriptionState State { get; set; }

        public bool IsTest => PurchaseToken.IsEmpty();

        public class UnderlayingType
        {
            public long EventTimeMillis { get; set; }
            public Subs Subscription { get; set; }

            public class Subs
            {
                public int NotificationType { get; set; }
                public string PurchaseToken { get; set; }
                public string SubscriptionId { get; set; }
            }

            public GooglePlayNotification ToNotification(string originalData) => new GooglePlayNotification
            {
                PurchaseToken = Subscription.PurchaseToken,
                ProductId = Subscription.SubscriptionId,
                EventTime = EventTimeMillis.ToDateTime(),
                OriginalData = originalData
            };
        }
    }
}
