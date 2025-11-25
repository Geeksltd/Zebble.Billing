namespace Zebble.Billing;

public partial record PurchaseAttemptResult(
    PurchaseAttemptStatus Status,
    string ProductId = null, string SubscriptionDate = null, string ExpirationDate = null,
    string OriginUserId = null, string NewUserId = null
);
