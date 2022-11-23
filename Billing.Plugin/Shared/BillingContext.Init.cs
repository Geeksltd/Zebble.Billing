namespace Zebble.Billing
{
    using Olive;
    using System;
    using System.Threading.Tasks;

    public partial class BillingContext
    {
        static BillingContextOptions Options;

        Func<IBillingUser> UserAccessor;
        TaskCompletionSource<bool> SubscriptionSource;

        public Task Initialization => SubscriptionSource?.Task;

        internal IBillingUser User => UserAccessor();

        Subscription subscription;
        internal Subscription Subscription
        {
            get => subscription;
            set
            {
                subscription = value;
                IsLoaded = true;
            }
        }

        public bool IsLoaded { get; private set; }

        internal IProductProvider ProductProvider { get; private set; }

        public static BillingContext Current { get; private set; }
        public static AsyncEvent<SubscriptionPurchasedEventArgs> SubscriptionPurchased = new();
        public static AsyncEvent<SubscriptionRestoredEventArgs> SubscriptionRestored = new();
        public static AsyncEvent<PriceUpdateFailedEventArgs> PriceUpdateFailed = new();

        public static void Initialize(Func<IBillingUser> userAccessor)
        {
            Initialize(new BillingContextOptions(), userAccessor);
        }

        public static void Initialize(BillingContextOptions options, Func<IBillingUser> userAccessor)
        {
            if (Current is not null)
            {
                Log.For<BillingContext>().Error($"Ignoring because {nameof(BillingContext)} is already initialized.");
                return;
            }

            if (options is null) throw new ArgumentNullException(nameof(options));
            if (userAccessor is null) throw new ArgumentNullException(nameof(userAccessor));

            Options = options.Validate();

            Current = new BillingContext
            {
                UserAccessor = userAccessor,
                ProductProvider = new ProductProvider(Options.CatalogPath),
                SubscriptionSource = new TaskCompletionSource<bool>()
            };

            Thread.Pool.RunOnNewThread(async () =>
            {
                await SubscriptionFileStore.Load();
                Current.SubscriptionSource.SetResult(true);
            });
        }
    }
}
