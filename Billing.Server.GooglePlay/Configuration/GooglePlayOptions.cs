namespace Zebble.Billing
{
    using System;

    public class GooglePlayOptions : StoreOptionsBase
    {
        public string ProjectId { get; set; }
        public string PrivateKeyId { get; set; }
        public string PrivateKey { get; set; }
        public string ClientEmail { get; set; }
        public string ClientId { get; set; }
        public string SubscriptionId { get; set; }
        public Uri QueueProcessorUri { get; set; }
    }
}
