namespace Zebble.Billing
{
    public static partial class BillingContext
    {
        internal static BillingContextOptions Options { get; private set; }

        public static IProductProvider ProductProvider { get; private set; }

        public static IBillingUser User { get; private set; }
        public static Subscription Subscription { get; private set; }

        public static AsyncEvent<SubscriptionPurchasedEventArgs> SubscriptionPurchased = new();
        public static AsyncEvent<SubscriptionRestoredEventArgs> SubscriptionRestored = new();
        public static AsyncEvent<VoucherAppliedEventArgs> VoucherApplied = new();

        public static void Initialize(BillingContextOptions options)
        {
            Options = options;

            ProductProvider = new ProductProvider(Options.CatalogPath);
        }

        public static void SetUser(IBillingUser user) => User = user;
    }
}
