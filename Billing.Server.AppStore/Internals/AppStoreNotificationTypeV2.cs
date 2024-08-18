namespace Zebble.Billing;

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
enum AppStoreNotificationTypeV2
{
    /// <summary>
    /// A notification type that indicates that the customer initiated a refund request for a consumable in-app purchase or auto-renewable subscription, and the App Store is requesting that you provide consumption data. For more information, see Send Consumption Information.
    /// </summary>
    [EnumMember(Value = "CONSUMPTION_REQUEST")]
    CONSUMPTION_REQUEST,

    /// <summary>
    /// A notification type that, along with its subtype, indicates that the customer made a change to their subscription plan. If the subtype is UPGRADE, the user upgraded their subscription. The upgrade goes into effect immediately, starting a new billing period, and the user receives a prorated refund for the unused portion of the previous period. If the subtype is DOWNGRADE, the customer downgraded their subscription. Downgrades take effect at the next renewal date and don’t affect the currently active plan.
    /// If the subtype is empty, the user changed their renewal preference back to the current subscription, effectively canceling a downgrade.
    /// For more information on subscription levels, see Ranking subscriptions within the group.
    /// </summary>
    [EnumMember(Value = "DID_CHANGE_RENEWAL_PREF")]
    DID_CHANGE_RENEWAL_PREF,

    /// <summary>
    /// A notification type that, along with its subtype, indicates that the customer made a change to the subscription renewal status. If the subtype is AUTO_RENEW_ENABLED, the customer reenabled subscription auto-renewal. If the subtype is AUTO_RENEW_DISABLED, the customer disabled subscription auto-renewal, or the App Store disabled subscription auto-renewal after the customer requested a refund.
    /// </summary>
    [EnumMember(Value = "DID_CHANGE_RENEWAL_STATUS")]
    DID_CHANGE_RENEWAL_STATUS,

    /// <summary>
    /// A notification type that, along with its subtype, indicates that the subscription failed to renew due to a billing issue. The subscription enters the billing retry period. If the subtype is GRACE_PERIOD, continue to provide service through the grace period. If the subtype is empty, the subscription isn’t in a grace period and you can stop providing the subscription service.
    /// Inform the customer that there may be an issue with their billing information. The App Store continues to retry billing for 60 days, or until the customer resolves their billing issue or cancels their subscription, whichever comes first. For more information, see Reducing Involuntary Subscriber Churn.
    /// </summary>
    [EnumMember(Value = "DID_FAIL_TO_RENEW")]
    DID_FAIL_TO_RENEW,

    /// <summary>
    /// A notification type that, along with its subtype, indicates that the subscription successfully renewed. If the subtype is BILLING_RECOVERY, the expired subscription that previously failed to renew has successfully renewed. If the subtype is empty, the active subscription has successfully auto-renewed for a new transaction period. Provide the customer with access to the subscription’s content or service.
    /// </summary>
    [EnumMember(Value = "DID_RENEW")]
    DID_RENEW,

    /// <summary>
    /// A notification type that, along with its subtype, indicates that a subscription expired. If the subtype is VOLUNTARY, the subscription expired after the user disabled subscription renewal. If the subtype is BILLING_RETRY, the subscription expired because the billing retry period ended without a successful billing transaction. If the subtype is PRICE_INCREASE, the subscription expired because the customer didn’t consent to a price increase that requires customer consent. If the subtype is PRODUCT_NOT_FOR_SALE, the subscription expired because the product wasn’t available for purchase at the time the subscription attempted to renew.
    /// A notification without a subtype indicates that the subscription expired for some other reason.
    /// </summary>
    [EnumMember(Value = "EXPIRED")]
    EXPIRED,

    /// <summary>
    /// A notification type that, along with its subtype UNREPORTED, indicates that Apple created an external purchase token for your app, but didn’t receive a report. For more information about reporting the token, see externalPurchaseToken.
    /// This notification applies only to apps that use the External Purchase API to provide alternative payment options.
    /// </summary>
    [EnumMember(Value = "EXTERNAL_PURCHASE_TOKEN")]
    EXTERNAL_PURCHASE_TOKEN,

    /// <summary>
    /// A notification type that indicates that the billing grace period has ended without renewing the subscription, so you can turn off access to the service or content. Inform the customer that there may be an issue with their billing information. The App Store continues to retry billing for 60 days, or until the customer resolves their billing issue or cancels their subscription, whichever comes first. For more information, see Reducing Involuntary Subscriber Churn.
    /// </summary>
    [EnumMember(Value = "GRACE_PERIOD_EXPIRED")]
    GRACE_PERIOD_EXPIRED,

    /// <summary>
    /// A notification type that, along with its subtype, indicates that the customer redeemed a promotional offer or offer code.
    /// If the subtype is INITIAL_BUY, the customer redeemed the offer for a first-time purchase. If the subtype is RESUBSCRIBE, the customer redeemed an offer to resubscribe to an inactive subscription. If the subtype is UPGRADE, the customer redeemed an offer to upgrade their active subscription, which goes into effect immediately. If the subtype is DOWNGRADE, the customer redeemed an offer to downgrade their active subscription, which goes into effect at the next renewal date. If the customer redeemed an offer for their active subscription, you receive an OFFER_REDEEMED notification type without a subtype.
    /// For more information about promotional offers, see Implementing promotional offers in your app. For more information about subscription offer codes, see Implementing offer codes in your app.
    /// </summary>
    [EnumMember(Value = "OFFER_REDEEMED")]
    OFFER_REDEEMED,

    /// <summary>
    /// The ONE_TIME_CHARGE notification is currently available only in the sandbox environment.
    /// A notification type that indicates the customer purchased a consumable, non-consumable, or non-renewing subscription. The App Store also sends this notification when the customer receives access to a non-consumable product through Family Sharing.
    /// For notifications about auto-renewable subscription purchases, see the SUBSCRIBED notification type.
    /// </summary>
    [EnumMember(Value = "ONE_TIME_CHARGE")]
    ONE_TIME_CHARGE,

    /// <summary>
    /// A notification type that, along with its subtype, indicates that the system has informed the customer of an auto-renewable subscription price increase.
    /// If the price increase requires customer consent, the subtype is PENDING if the customer hasn’t responded to the price increase, or ACCEPTED if the customer has consented to the price increase.
    /// If the price increase doesn’t require customer consent, the subtype is ACCEPTED.
    /// For information about how the system calls your app before it displays the price consent sheet for subscription price increases that require customer consent, see paymentQueueShouldShowPriceConsent(_:). For information about managing subscription prices, see Managing Price Increases for Auto-Renewable Subscriptions and Managing Prices.
    /// </summary>
    [EnumMember(Value = "PRICE_INCREASE")]
    PRICE_INCREASE,

    /// <summary>
    /// A notification type that indicates that the App Store successfully refunded a transaction for a consumable in-app purchase, a non-consumable in-app purchase, an auto-renewable subscription, or a non-renewing subscription.
    /// The revocationDate contains the timestamp of the refunded transaction. The originalTransactionId and productId identify the original transaction and product. The revocationReason contains the reason.
    /// To request a list of all refunded transactions for a customer, see Get Refund History in the App Store Server API.
    /// </summary>
    [EnumMember(Value = "REFUND")]
    REFUND,

    /// <summary>
    /// A notification type that indicates the App Store declined a refund request.
    /// </summary>
    [EnumMember(Value = "REFUND_DECLINED")]
    REFUND_DECLINED,

    /// <summary>
    /// A notification type that indicates the App Store reversed a previously granted refund due to a dispute that the customer raised. If your app revoked content or services as a result of the related refund, it needs to reinstate them.
    /// This notification type can apply to any in-app purchase type: consumable, non-consumable, non-renewing subscription, and auto-renewable subscription. For auto-renewable subscriptions, the renewal date remains unchanged when the App Store reverses a refund.
    /// </summary>
    [EnumMember(Value = "REFUND_REVERSED")]
    REFUND_REVERSED,

    /// <summary>
    /// A notification type that indicates the App Store extended the subscription renewal date for a specific subscription. You request subscription-renewal-date extensions by calling Extend a Subscription Renewal Date or Extend Subscription Renewal Dates for All Active Subscribers in the App Store Server API.
    /// </summary>
    [EnumMember(Value = "RENEWAL_EXTENDED")]
    RENEWAL_EXTENDED,

    /// <summary>
    /// A notification type that indicates that an in-app purchase the customer was entitled to through Family Sharing is no longer available through sharing. The App Store sends this notification when a purchaser disables Family Sharing for their purchase, the purchaser (or family member) leaves the family group, or the purchaser receives a refund. Your app also receives a paymentQueue(_:didRevokeEntitlementsForProductIdentifiers:) call. Family Sharing applies to non-consumable in-app purchases and auto-renewable subscriptions. For more information about Family Sharing, see Supporting Family Sharing in your app.
    /// </summary>
    [EnumMember(Value = "REVOKE")]
    REVOKE,

    /// <summary>
    /// A notification type that, along with its subtype, indicates that the customer subscribed to an auto-renewable subscription. If the subtype is INITIAL_BUY, the customer either purchased or received access through Family Sharing to the subscription for the first time. If the subtype is RESUBSCRIBE, the user resubscribed or received access through Family Sharing to the same subscription or to another subscription within the same subscription group.
    /// For notifications about other product type purchases, see the ONE_TIME_CHARGE notification type.
    /// </summary>
    [EnumMember(Value = "SUBSCRIBED")]
    SUBSCRIBED,

    /// <summary>
    /// A notification type that the App Store server sends when you request it by calling the Request a Test Notification endpoint. Call that endpoint to test whether your server is receiving notifications. You receive this notification only at your request. For troubleshooting information, see the Get Test Notification Status endpoint.
    /// </summary>
    [EnumMember(Value = "SUBSCRIBED")]
    TEST
}
