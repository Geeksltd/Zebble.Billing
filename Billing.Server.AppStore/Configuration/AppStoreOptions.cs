namespace Zebble.Billing
{
    using System;
    using Apple.Receipt.Verificator.Models;

    public class AppStoreOptions : StoreOptionsBase
    {
        public string SharedSecret { get; set; }
        public AppStoreEnvironment Environment { get; set; }
        public Uri HookInterceptorUri { get; set; }

        internal void Apply(AppleReceiptVerificationSettings @that)
        {
            @that.VerifyReceiptSharedSecret = SharedSecret;
            @that.VerificationType = Environment.ToVerificationType();
            @that.AllowedBundleIds = new[] { PackageName };
        }
    }
}
