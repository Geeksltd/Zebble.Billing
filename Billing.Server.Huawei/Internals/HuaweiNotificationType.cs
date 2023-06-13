namespace Zebble.Billing
{
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    enum HuaweiNotificationType
    {
        /// <summary>
        /// Indicates that either Apple customer support canceled the subscription or the user upgraded their subscription. The CancellationDate contains the date and time of the change.
        /// </summary>
        [EnumMember(Value = "CANCEL")]
        CanceledOrUpgraded,

        /// <summary>
        /// Indicates that the customer made a change in their subscription plan that takes effect at the next renewal. The currently active plan isn't affected.
        /// </summary>
        [EnumMember(Value = "DID_CHANGE_RENEWAL_PREF")]
        RenewalPreferencesChanged,

        /// <summary>
        /// Indicates a change in the subscription renewal status. Check AutoRenewStatusChangeDate to know the date and time of the last status update. Check AutoRenewStatus to know the current renewal status.
        /// </summary>
        [EnumMember(Value = "DID_CHANGE_RENEWAL_STATUS")]
        RenewalStatusChanged,

        /// <summary>
        /// Indicates a subscription that failed to renew due to a billing issue. Check IsInBillingRetryPeriod to know the current retry status of the subscription. Check GracePeriodExpiresDate to know the new service expiration date if the subscription is in a billing grace period.
        /// </summary>
        [EnumMember(Value = "DID_FAIL_TO_RENEW")]
        AutoRenewFailed,

        /// <summary>
        /// Indicates a successful automatic renewal of an expired subscription that failed to renew in the past. Check ExpiresDate to determine the next renewal date and time.
        /// </summary>
        [EnumMember(Value = "DID_RECOVER")]
        AutoRecovered,

        /// <summary>
        /// Indicates that a customer's subscription has successfully auto-renewed for a new transaction period.
        /// </summary>
        [EnumMember(Value = "DID_RENEW")]
        AutoRenewed,

        /// <summary>
        /// Occurs at the user's initial purchase of the subscription. Store LatestReceipt on your server as a token to verify the user's subscription status at any time by validating it with the App Store.
        /// </summary>
        [EnumMember(Value = "INITIAL_BUY")]
        InitialBuy,

        /// <summary>
        /// Indicates the customer renewed a subscription interactively, either by using your app's interface, or on the App Store in the account's Subscriptions settings. Make service available immediately.
        /// </summary>
        [EnumMember(Value = "INTERACTIVE_RENEWAL")]
        InteractivelyRenewed,

        /// <summary>
        /// Indicates that App Store has started asking the customer to consent to your app's subscription price increase. In the UnifiedReceipt.PendingRenewalInfoObject, the PriceConsentStatus value is false, indicating that App Store is asking for the customer's consent, and hasn't received it. The subscription won't auto-renew unless the user agrees to the new price. When the customer agrees to the price increase, the system sets PriceConsentStatus to true. Check the receipt using VerifyReceipt to view the updated price-consent status.
        /// </summary>
        [EnumMember(Value = "PRICE_INCREASE_CONSENT")]
        PriceIncreaseConsent,

        /// <summary>
        /// Indicates that App Store successfully refunded a transaction. The CancellationDate contains the time of the refunded transaction. The OriginalTransactionId and ProductId identify the original transaction and product. The CancellationReason contains the reason.
        /// </summary>
        [EnumMember(Value = "REFUND")]
        Refunded,

        /// <summary>
        /// Indicates that an in-app purchase the user was entitled to through Family Sharing is no longer available through sharing. StoreKit sends this notification when a purchaser disabled Family Sharing for a product, the purchaser (or family member) left the family group, or the purchaser asked for and received a refund.
        /// </summary>
        [EnumMember(Value = "REVOKE")]
        FamilySharingRevoked
    }
}
