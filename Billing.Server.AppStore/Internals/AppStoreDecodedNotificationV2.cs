using System;

namespace Zebble.Billing;

record AppStoreDecodedNotificationV2(
    AppStoreNotificationTypeV2 NotificationType,
    AppStoreNotificationSubtypeV2 Subtype,
    Guid NotificationUuid,
    Version Version,
    AppStoreDecodedNotificationDataV2 Data)
{
    /// <summary>
    /// Original notification json
    /// </summary>
    public string OriginalData { get; private set; }

    public AppStoreDecodedNotificationV2 WithOriginalData(string originalData)
    {
        OriginalData = originalData;
        return this;
    }

    public SubscriptionInfoArgs ToArgs() => new()
    {
        PackageName = Data.BundleId,
        OriginalTransactionId = Data.SignedTransactionInfo.OriginalTransactionId
    };
}