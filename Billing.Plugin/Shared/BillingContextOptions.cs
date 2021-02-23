namespace Zebble.Billing
{
    using System;
    using Olive;

    public class BillingContextOptions
    {
        /// <summary>
        /// The base uri to your Zebble.Billing.Server app.
        /// </summary>
        public Uri BaseUri { get; set; }

        /// <summary>
        /// The relative path to purchase attempt endpoint. 
        /// </summary>
        /// <remarks>
        /// The default value is "app/purchase-attempt".
        /// </remarks>
        public string PurchaseAttemptPath { get; set; } = "app/purchase-attempt";

        /// <summary>
        /// The relative path to subscription status endpoint. 
        /// </summary>
        /// <remarks>
        /// The default value is "app/subscription-status".
        /// </remarks>
        public string SubscriptionStatusPath { get; set; } = "app/subscription-status";

        /// <summary>
        /// The path of the catalog file in the client app.
        /// </summary>
        /// <remarks>
        /// The default value is "Catalog.json".
        /// </remarks>
        public string CatalogPath { get; set; } = @"Catalog.json";

        internal void Validate()
        {
            if (BaseUri == null) throw new ArgumentNullException(nameof(BaseUri));
            if (!BaseUri.IsAbsoluteUri) throw new ArgumentException($"{nameof(BaseUri)} should be absolute.");

            if (PurchaseAttemptPath.IsEmpty()) throw new ArgumentNullException(nameof(PurchaseAttemptPath));

            if (SubscriptionStatusPath.IsEmpty()) throw new ArgumentNullException(nameof(SubscriptionStatusPath));

            if (CatalogPath.IsEmpty()) throw new ArgumentNullException(nameof(CatalogPath));
        }
    }
}
