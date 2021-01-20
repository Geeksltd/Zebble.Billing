﻿namespace Zebble.Billing
{
    using Olive;
    using System;

    public static partial class BillingContext
    {
        public static string BaseUrl { get; private set; }

        public static IBillingUser User { get; private set; }

        public static AsyncEvent<PurchaseRecognizedEventArgs> PurchaseRecognized = new();

        public static void Initialize(string baseUrl, IBillingUser user, params Product[] products)
        {
            BaseUrl = baseUrl.OrNullIfEmpty() ?? throw new ArgumentNullException(nameof(baseUrl));

            User = user ?? throw new ArgumentNullException(nameof(user));

            if (products.None()) throw new ArgumentException("At least one product should be specified.", nameof(products));

            products.Do(ProductsCache.RegisteredProducts.Add);
        }

        public static bool ArePricesUpToDate() => ProductsCache.ArePricesUpToDate();
    }
}
