namespace Zebble.Billing
{
    using Olive;
    using System;

    public class AppStoreOptions : StoreOptionsBase
    {
        public string SharedSecret { get; set; }
        public Uri HookInterceptorUri { get; set; }

        internal new bool Validate()
        {
            if (SharedSecret.IsEmpty()) throw new ArgumentNullException(nameof(SharedSecret));

            if (HookInterceptorUri == null) throw new ArgumentNullException(nameof(HookInterceptorUri));
            if (HookInterceptorUri.IsAbsoluteUri == false) throw new InvalidOperationException($"{nameof(HookInterceptorUri)} should be absolute.");

            return base.Validate();
        }
    }
}
