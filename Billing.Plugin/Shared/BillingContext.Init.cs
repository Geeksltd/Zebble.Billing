namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;

    public partial class BillingContext
    {
        static BillingContextOptions Options;

        Func<IBillingUser> UserAccessor;
        TaskCompletionSource<bool> SubscriptionSource;

        public Task Initialization => SubscriptionSource?.Task;

        internal IBillingUser User => UserAccessor();
        internal Subscription Subscription { get; set; }
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

            Current.SubscriptionSource = new TaskCompletionSource<bool>();
            Thread.Pool.RunOnNewThread(async () =>
            {
                await SubscriptionFileStore.Load();
                Current.SubscriptionSource.SetResult(true);
            });
        }
    }
}
