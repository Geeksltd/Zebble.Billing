namespace Zebble.Billing
{
    using System;

    public partial class BillingContext
    {
        static BillingContextOptions Options;
        IBillingUser User;
        Subscription Subscription;

        internal IProductProvider ProductProvider { get; private set; }

        public static BillingContext Current { get; private set; }
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
