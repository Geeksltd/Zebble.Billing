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
        /// The relative path to verify purchase endpoint. 
        /// </summary>
        /// <remarks>
        /// The default value is "app/verify-purchase".
        /// </remarks>
        public string VerifyPurchasePath { get; set; } = "app/verify-purchase";

        /// <summary>
        /// The relative path to purchase attempt endpoint. 
        /// </summary>
        /// <remarks>
        /// The default value is "app/purchase-attempt".
        /// </remarks>
        public string PurchaseAttemptPath { get; set; } = "app/purchase-attempt";

        /// <summary>
        /// The relative path to voucher apply endpoint. 
        /// </summary>
        /// <remarks>
        /// The default value is "voucher/apply".
        /// </remarks>
        public string VoucherApplyPath { get; set; } = "voucher/apply";

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

        public BillingContextOptions Validate()
        {
            if (BaseUri == null)
            {
                var baseUrl = Config.Get("Billing.Base.Url", Config.Get("Api.Base.Url"));
                if (baseUrl.HasValue()) BaseUri = new Uri(baseUrl);
            }

            if (BaseUri == null)
                throw new ArgumentNullException(nameof(BaseUri), "Add Billing.Base.Url or Api.Base.Url to your Config.xml");

            if (!BaseUri.IsAbsoluteUri) throw new ArgumentException($"{nameof(BaseUri)} should be absolute.");

            if (PurchaseAttemptPath.IsEmpty()) throw new ArgumentNullException(nameof(PurchaseAttemptPath));

            if (VoucherApplyPath.IsEmpty()) throw new ArgumentNullException(nameof(VoucherApplyPath));

            if (SubscriptionStatusPath.IsEmpty()) throw new ArgumentNullException(nameof(SubscriptionStatusPath));

            if (CatalogPath.IsEmpty()) throw new ArgumentNullException(nameof(CatalogPath));

            return this;
        }
    }
}
