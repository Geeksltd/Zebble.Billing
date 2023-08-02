namespace Zebble.Billing
{
    using Olive;
    using System;

    public partial class BillingContext
    {
        static BillingContextOptions Options;

        Subscription subscription;
        internal Subscription Subscription
        {
            get => subscription;
            set => subscription = value;
        }

        public bool IsLoaded { get; internal set; }

        internal IProductProvider ProductProvider { get; private set; }

        public static BillingContext Current { get; private set; }
        public static AsyncEvent<SubscriptionPurchasedEventArgs> SubscriptionPurchased = new();
        public static AsyncEvent<SubscriptionRestoredEventArgs> SubscriptionRestored = new();
        public static AsyncEvent PriceUpdated = new();
        public static AsyncEvent<PriceUpdateFailedEventArgs> PriceUpdateFailed = new();

        public static void Initialize(IBillingUser user)
        {
            Initialize(new BillingContextOptions(), user);
        }

        public static void Initialize(BillingContextOptions options, IBillingUser user)
        {
            if (Current is not null)
            {
                Log.For<BillingContext>().Error($"Ignoring because {nameof(BillingContext)} is already initialized.");
                return;
            }

            if (user is null) throw new ArgumentNullException(nameof(user));
            if (options is null) throw new ArgumentNullException(nameof(options));

            Options = options.Validate();

            Current = new BillingContext
            {
                ProductProvider = new ProductProvider(Options.CatalogPath)
            };

            SubscriptionFileStore.Load(user);
        }
    }
}
