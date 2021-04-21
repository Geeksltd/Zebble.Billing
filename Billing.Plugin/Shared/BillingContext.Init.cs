namespace Zebble.Billing
{
    using System;

    public partial class BillingContext
    {
        static BillingContextOptions Options;

        Func<IBillingUser> UserAccessor;
        internal IBillingUser User => UserAccessor();
        internal Subscription Subscription { get; private set; }
        internal IProductProvider ProductProvider { get; private set; }

        public static BillingContext Current { get; private set; }
        public static AsyncEvent<SubscriptionPurchasedEventArgs> SubscriptionPurchased = new();
        public static AsyncEvent<SubscriptionRestoredEventArgs> SubscriptionRestored = new();

        public static void Initialize(Func<IBillingUser> userAccessor)
        {
            Initialize(new BillingContextOptions(), userAccessor);
        }

        public static void Initialize(BillingContextOptions options, Func<IBillingUser> userAccessor)
        {
            if (Current != null) throw new InvalidOperationException($"{nameof(BillingContext)} is already initialized.");

            if (options is null) throw new ArgumentNullException(nameof(options));
            if (userAccessor is null) throw new ArgumentNullException(nameof(userAccessor));

            Options = options;

            Current = new BillingContext
            {
                UserAccessor = userAccessor,
                ProductProvider = new ProductProvider(Options.CatalogPath)
            };
        }
    }
}
