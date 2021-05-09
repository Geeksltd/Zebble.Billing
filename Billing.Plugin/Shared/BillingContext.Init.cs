namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;

    public partial class BillingContext
    {
        static BillingContextOptions Options;

        Func<IBillingUser> UserAccessor;
        internal IBillingUser User => UserAccessor();
        internal Subscription Subscription { get; set; }
        internal IProductProvider ProductProvider { get; private set; }

        public static BillingContext Current { get; private set; }
        public static AsyncEvent<SubscriptionPurchasedEventArgs> SubscriptionPurchased = new();
        public static AsyncEvent<SubscriptionRestoredEventArgs> SubscriptionRestored = new();

        public static Task Initialize(Func<IBillingUser> userAccessor)
        {
            return Initialize(new BillingContextOptions(), userAccessor);
        }

        public static async Task Initialize(BillingContextOptions options, Func<IBillingUser> userAccessor)
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

            await SubscriptionFileStore.Load();
        }
    }
}
