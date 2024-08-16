using System;

namespace Zebble.Billing;

record AppStoreDecodedNotification(
    AppStoreNotificationType NotificationType,
    AppStoreNotificationSubtype Subtype,
    Guid NotificationUuid,
    Version Version,
    AppStoreDecodedNotificationData Data)
{
    /// <summary>
    /// Original notification json
    /// </summary>
    public string OriginalData { get; private set; }

    public AppStoreDecodedNotification WithOriginalData(string originalData)
    {
        OriginalData = originalData;
        return this;
    }

    public SubscriptionInfoArgs ToArgs() => new()
    {
        OriginalTransactionId = Data.SignedTransactionInfo.OriginalTransactionId
    };
}