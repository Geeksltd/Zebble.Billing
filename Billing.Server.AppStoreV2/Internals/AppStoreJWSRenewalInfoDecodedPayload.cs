using System;

namespace Zebble.Billing;

record AppStoreJWSRenewalInfoDecodedPayload(string AutoRenewProductId, int AutoRenewStatus,
    string Environment, int ExpirationIntent, DateTimeOffset GracePeriodExpiresDate,
    bool IsInBillingRetryPeriod, string OfferIdentifier, int OfferType, string OriginalTransactionId,
    int PriceIncreaseStatus, string ProductId, DateTimeOffset RecentSubscriptionStartDate, 
    DateTimeOffset RenewalDate, DateTimeOffset SignedDate);
