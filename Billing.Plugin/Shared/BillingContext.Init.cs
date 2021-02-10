namespace Zebble.Billing
{
    using System;

    public partial class BillingContext
    {
        public static BillingContext Current { get; private set; }

        internal static BillingContextOptions Options { get; private set; }

        public IProductProvider ProductProvider { get; private set; }

        public IBillingUser User { get; private set; }
        public Subscription Subscription { get; private set; }

        public static AsyncEvent<SubscriptionPurchasedEventArgs> SubscriptionPurchased = new();
        public static AsyncEvent<SubscriptionRestoredEventArgs> SubscriptionRestored = new();

        public static void Initialize(BillingContextOptions options)
        {
            if (Current != null) throw new InvalidOperationException($"{nameof(BillingContext)} is already initialized.");

            Options = options;

            Current = new BillingContext
            {
                ProductProvider = new ProductProvider(Options.CatalogPath)
            };
        }

        public void SetUser(IBillingUser user) => User = user;
    }
}
