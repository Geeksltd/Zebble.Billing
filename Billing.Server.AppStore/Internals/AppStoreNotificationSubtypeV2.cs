namespace Zebble.Billing;

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
enum AppStoreNotificationSubtypeV2
{
    /// <summary>
    /// Applies to the PRICE_INCREASE notificationType. A notification with this subtype indicates that the customer consented to the subscription price increase if the price increase requires customer consent, or that the system notified them of a price increase if the price increase doesn't require customer consent.
    /// </summary>
    [EnumMember(Value = "ACCEPTED")]
    ACCEPTED,

    /// <summary>
    /// Applies to the DID_CHANGE_RENEWAL_STATUS notificationType. A notification with this subtype indicates that the user disabled subscription auto-renewal, or the App Store disabled subscription auto-renewal after the user requested a refund.
    /// </summary>
    [EnumMember(Value = "AUTO_RENEW_DISABLED")]
    AUTO_RENEW_DISABLED,

    /// <summary>
    /// Applies to the DID_CHANGE_RENEWAL_STATUS notificationType. A notification with this subtype indicates that the user enabled subscription auto-renewal.
    /// </summary>
    [EnumMember(Value = "AUTO_RENEW_ENABLED")]
    AUTO_RENEW_ENABLED,

    /// <summary>
    /// Applies to the DID_RENEW notificationType. A notification with this subtype indicates that the expired subscription that previously failed to renew has successfully renewed.
    /// </summary>
    [EnumMember(Value = "BILLING_RECOVERY")]
    BILLING_RECOVERY,

    /// <summary>
    /// Applies to the EXPIRED notificationType. A notification with this subtype indicates that the subscription expired because the subscription failed to renew before the billing retry period ended.
    /// </summary>
    [EnumMember(Value = "BILLING_RETRY")]
    BILLING_RETRY,

    /// <summary>
    /// Applies to the DID_CHANGE_RENEWAL_PREF notificationType. A notification with this subtype indicates that the user downgraded their subscription or cross-graded to a subscription with a different duration. Downgrades take effect at the next renewal date.
    /// </summary>
    [EnumMember(Value = "DOWNGRADE")]
    DOWNGRADE,

    /// <summary>
    /// Applies to the RENEWAL_EXTENSION notificationType. A notification with this subtype indicates that the subscription-renewal-date extension failed for an individual subscription. For details, see the data object in the responseBodyV2DecodedPayload. For information on the request, see Extend Subscription Renewal Dates for All Active Subscribers.
    /// </summary>
    [EnumMember(Value = "FAILURE")]
    FAILURE,

    /// <summary>
    /// Applies to the DID_FAIL_TO_RENEW notificationType. A notification with this subtype indicates that the subscription failed to renew due to a billing issue. Continue to provide access to the subscription during the grace period.
    /// </summary>
    [EnumMember(Value = "GRACE_PERIOD")]
    GRACE_PERIOD,

    /// <summary>
    /// Applies to the SUBSCRIBED notificationType. A notification with this subtype indicates that the user purchased the subscription for the first time or that the user received access to the subscription through Family Sharing for the first time.
    /// </summary>
    [EnumMember(Value = "INITIAL_BUY")]
    INITIAL_BUY,

    /// <summary>
    /// Applies to the PRICE_INCREASE notificationType. A notification with this subtype indicates that the system informed the user of the subscription price increase, but the user hasn’t accepted it.
    /// </summary>
    [EnumMember(Value = "PENDING")]
    PENDING,

    /// <summary>
    /// Applies to the EXPIRED notificationType. A notification with this subtype indicates that the subscription expired because the user didn’t consent to a price increase.
    /// </summary>
    [EnumMember(Value = "PRICE_INCREASE")]
    PRICE_INCREASE,

    /// <summary>
    /// Applies to the EXPIRED notificationType. A notification with this subtype indicates that the subscription expired because the product wasn’t available for purchase at the time the subscription attempted to renew.
    /// </summary>
    [EnumMember(Value = "PRODUCT_NOT_FOR_SALE")]
    PRODUCT_NOT_FOR_SALE,

    /// <summary>
    /// Applies to the SUBSCRIBED notificationType. A notification with this subtype indicates that the user resubscribed or received access through Family Sharing to the same subscription or to another subscription within the same subscription group.
    /// </summary>
    [EnumMember(Value = "RESUBSCRIBE")]
    RESUBSCRIBE,

    /// <summary>
    /// Applies to the RENEWAL_EXTENSION notificationType. A notification with this subtype indicates that the App Store server completed your request to extend the subscription renewal date for all eligible subscribers. For the summary details, see the summary object in the responseBodyV2DecodedPayload. For information on the request, see Extend Subscription Renewal Dates for All Active Subscribers.
    /// </summary>
    [EnumMember(Value = "SUMMARY")]
    SUMMARY,

    /// <summary>
    /// Applies to the DID_CHANGE_RENEWAL_PREF notificationType. A notification with this subtype indicates that the user upgraded their subscription or cross-graded to a subscription with the same duration. Upgrades take effect immediately.
    /// </summary>
    [EnumMember(Value = "UPGRADE")]
    UPGRADE,

    /// <summary>
    /// Applies to the EXTERNAL_PURCHASE_TOKEN notificationType. A notification with this subtype indicates that Apple created a token for your app but didn't receive a report. For more information about reporting the token, see externalPurchaseToken.
    /// </summary>
    [EnumMember(Value = "UNREPORTED")]
    UNREPORTED,

    /// <summary>
    /// Applies to the EXPIRED notificationType. A notification with this subtype indicates that the subscription expired after the user disabled subscription auto-renewal.
    /// </summary>
    [EnumMember(Value = "VOLUNTARY")]
    VOLUNTARY
}
