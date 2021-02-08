namespace Zebble.Billing
{
    using System;

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
        /// The relative path to voucher's apply endpoint. 
        /// </summary>
        /// <remarks>
        /// The default value is "voucher/apply".
        /// </remarks>
        public string VoucherApplyPath { get; set; } = "voucher/apply";

        /// <summary>
        /// The path of the catalog file in the client app. The default is "Resources\Catalog.json".
        /// </summary>
        /// <remarks>
        /// The default value is "Resources\Catalog.json".
        /// </remarks>
        public string CatalogPath { get; set; } = @"Resources\Catalog.json";
    }
}
