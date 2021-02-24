namespace Zebble.Billing
{
    using System;
    using Olive;

    public partial class BillingContext
    {
        static BillingContextOptions Options;

        internal IBillingUser User { get; private set; }
        internal Subscription Subscription { get; private set; }
        internal IProductProvider ProductProvider { get; private set; }

        public static BillingContext Current { get; private set; }
        public static AsyncEvent<SubscriptionPurchasedEventArgs> SubscriptionPurchased = new();
        public static AsyncEvent<SubscriptionRestoredEventArgs> SubscriptionRestored = new();

        public static void Initialize(BillingContextOptions options = null)
        {
            if (Current != null) throw new InvalidOperationException($"{nameof(BillingContext)} is already initialized.");

            Options = options ?? new BillingContextOptions
            {
                BaseUri = new Uri(Config.Get("Billing.Base.Url").OrNullIfEmpty() ?? throw new Exception("Add Billing.Base.Url to your Config.xml."))
            };

            Options.Validate();

            Current = new BillingContext
            {
                ProductProvider = new ProductProvider(Options.CatalogPath)
            };
        }

        public void SetUser(IBillingUser user) => User = user;
    }
}
