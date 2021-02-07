namespace Zebble.Billing
{
    using System;
    using Apple.Receipt.Verificator.Models;
    using Olive;

    public class AppStoreOptions : StoreOptionsBase
    {
        public string SharedSecret { get; set; }
        public AppStoreEnvironment Environment { get; set; }
        public Uri HookInterceptorUri { get; set; }

        internal new bool Validate()
        {
            if (SharedSecret.IsEmpty()) throw new ArgumentNullException(nameof(SharedSecret));

            if (HookInterceptorUri == null) throw new ArgumentNullException(nameof(HookInterceptorUri));
            if (HookInterceptorUri.IsAbsoluteUri == false) throw new InvalidOperationException($"{nameof(HookInterceptorUri)} should be absolute.");

            return base.Validate();
        }

        internal void Apply(AppleReceiptVerificationSettings @that)
        {
            @that.VerifyReceiptSharedSecret = SharedSecret;
            @that.VerificationType = Environment.ToVerificationType();
            @that.AllowedBundleIds = new[] { PackageName };
        }
    }
}
