namespace Zebble.Billing
{
    using Olive;
    using System;

    public static partial class BillingContext
    {
        public static string BaseUrl { get; private set; }

        public static IProductProvider ProductProvider { get; private set; }

        public static IBillingUser User { get; private set; }
        public static Subscription Subscription { get; private set; }

        public static AsyncEvent<SubscriptionPurchasedEventArgs> SubscriptionPurchased = new();
        public static AsyncEvent<SubscriptionRestoredEventArgs> SubscriptionRestored = new();
        public static AsyncEvent<VoucherAppliedEventArgs> VoucherApplied = new();

        public static void Initialize(string baseUrl, string catalogPath = @"Resources\Catalog.json")
        {
            BaseUrl = baseUrl.OrNullIfEmpty() ?? throw new ArgumentNullException(nameof(baseUrl));
            ProductProvider = new ProductProvider(catalogPath);
        }

        public static void SetUser(IBillingUser user) => User = user;
    }
}
