namespace Zebble.Billing
{
    using System;

    public class AppStoreOptions : StoreOptionsBase
    {
        public string SharedSecret { get; set; }
        public Uri HookInterceptorUri { get; set; }
    }
}
