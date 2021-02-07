namespace Zebble.Billing
{
    using System;
    using System.Text.Json.Serialization;

    class AppStorePendingRenewaInfo
    {
        /// <summary>
        /// A flag that indicates Apple is attempting to renew an expired subscription automatically. This field is only present if an auto-renewable subscription is in the billing retry state.
        /// </summary>
        [JsonPropertyName("is_in_billing_retry_period")]
        public bool? IsInBillingRetryPeriod { get; set; }

        /// <summary>
        /// The time at which the grace period for subscription renewals expires. This key is only present for apps that have Billing Grace Period enabled and when the user experiences a billing error at the time of renewal.
        /// </summary>
        [JsonPropertyName("grace_period_expires_date_ms")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? GracePeriodExpirationDate { get; set; }
    }
}
