namespace Zebble.Billing
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Apple.Receipt.Models;
    using Apple.Receipt.Models.Enums;
    using Apple.Receipt.Verificator.Models;
    using Apple.Receipt.Verificator.Models.IAPVerification;
    using Apple.Receipt.Verificator.Services;
    using Microsoft.Extensions.Logging;
    using Olive;

    class AppStoreConnector : IStoreConnector
    {
        readonly ILogger<AppStoreConnector> Logger;
        readonly IAppleReceiptVerificatorService Verificator;
        readonly ISubscriptionRepository Repository;

        public AppStoreConnector(ILogger<AppStoreConnector> logger, IAppleReceiptVerificatorService verificator, ISubscriptionRepository repository)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Verificator = verificator ?? throw new ArgumentNullException(nameof(verificator));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<PurchaseVerificationStatus> VerifyPurchase(VerifyPurchaseArgs args)
        {
            var (_, status) = await GetVerifiedResult(args.UserId, args.ReceiptData);

            return status;
        }

        public async Task<SubscriptionInfo> GetSubscriptionInfo(SubscriptionInfoArgs args)
        {
            var (result, status) = await GetVerifiedResult(args.UserId, args.ReceiptData);

            if (status != PurchaseVerificationStatus.Verified) return null;

            return CreateSubscription(result.AppleVerificationResponse.LatestReceiptInfo.First());
        }

        SubscriptionInfo CreateSubscription(AppleInAppPurchaseReceipt purchase)
        {
            return new SubscriptionInfo
            {
                UserId = null,
                TransactionId = purchase.OriginalTransactionId,
                SubscriptionDate = purchase.PurchaseDateDt?.ToUniversal(),
                ExpirationDate = purchase.ExpirationDateDt?.ToUniversal(),
                CancellationDate = purchase.CancellationDateDt?.ToUniversal(),
                AutoRenews = purchase.SubscriptionAutoRenewStatus == AppleSubscriptionAutoRenewStatus.Active
            };
        }

        async Task<(AppleReceiptVerificationResult, PurchaseVerificationStatus)> GetVerifiedResult(string userId, string receiptData)
        {
            var result = await Verificator.VerifyAppleReceiptAsync(receiptData);

            if (result is null) return (null, PurchaseVerificationStatus.Failed);

            return (result, await ValidateVerificationResult(userId, result));
        }

        async Task<PurchaseVerificationStatus> ValidateVerificationResult(string userId, AppleReceiptVerificationResult verificationResult)
        {
            if (verificationResult.AppleVerificationResponse is null)
            {
                Logger.LogWarning($"{verificationResult.Message}.");
                return PurchaseVerificationStatus.Failed;
            }

            if (verificationResult.AppleVerificationResponse.StatusCode != IAPVerificationResponseStatus.Ok)
            {
                Logger.LogWarning($"{verificationResult.Message} [{verificationResult.AppleVerificationResponse.Status} - {verificationResult.AppleVerificationResponse.StatusCode}]");
                return PurchaseVerificationStatus.Failed;
            }

            if (userId.IsEmpty()) return PurchaseVerificationStatus.Verified;

            var transactionIds = verificationResult.AppleVerificationResponse
                .LatestReceiptInfo
                .Select(x => x.TransactionId)
                .ToArray();
            if (transactionIds.None()) return PurchaseVerificationStatus.Verified;

            var originUserId = await Repository.GetOriginUserOfTransactionIds(transactionIds);
            if (originUserId.IsEmpty()) return PurchaseVerificationStatus.Verified;

            if (!userId.Equals(originUserId, caseSensitive: false))
            {
                Logger.LogWarning($"This receipt is associated to {originUserId} and can't be used for {userId}.");
                return PurchaseVerificationStatus.UserMismatched;
            }

            return PurchaseVerificationStatus.Verified;
        }
    }
}
