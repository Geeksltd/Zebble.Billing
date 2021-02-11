namespace Zebble.Billing
{
    using System;

    public partial class BillingContext<T> where T : Product
    {
        static BillingContextOptions Options;
        IBillingUser User;
        Subscription Subscription;

        internal IProductProvider<T> ProductProvider { get; private set; }

        public static BillingContext<T> Current { get; private set; }
        public static AsyncEvent<SubscriptionPurchasedEventArgs<T>> SubscriptionPurchased = new();
        public static AsyncEvent<SubscriptionRestoredEventArgs<T>> SubscriptionRestored = new();

        public static void Initialize(BillingContextOptions options)
        {
            if (Current != null) throw new InvalidOperationException($"{nameof(BillingContext<T>)} is already initialized.");

            Options = options;

            Current = new BillingContext<T>
            {
                ProductProvider = new ProductProvider<T>(Options.CatalogPath)
            };
        }

        public void SetUser(IBillingUser user) => User = user;
    }
}
