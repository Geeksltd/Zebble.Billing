namespace Zebble.Billing
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
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

        public async Task<SubscriptionInfo> GetSubscriptionInfo(SubscriptionInfoArgs args)
        {
            var (result, status) = await GetVerifiedResult(args.UserId, args.PurchaseToken);

            return status switch
            {
                SubscriptionQueryStatus.NotFound => SubscriptionInfo.NotFound,
                SubscriptionQueryStatus.UserMismatched => SubscriptionInfo.UserMismatched,
                _ => CreateSubscription(args.ProductId, result.AppleVerificationResponse)
            };
        }

        static SubscriptionInfo CreateSubscription(string productId, IAPVerificationResponse response)
        {
            var purchase = response?.LatestReceiptInfo.OrderBy(x => x.PurchaseDateDt).LastOrDefault(x => x.ProductId == productId);
            if (purchase is null) return SubscriptionInfo.NotFound;

            return new SubscriptionInfo
            {
                TransactionId = purchase.OriginalTransactionId,
                SubscriptionDate = purchase.PurchaseDateDt,
                ExpirationDate = purchase.ExpirationDateDt,
                CancellationDate = purchase.CancellationDateDt,
                AutoRenews = purchase.SubscriptionAutoRenewStatus == AppleSubscriptionAutoRenewStatus.Active
            };
        }

        async Task<(AppleReceiptVerificationResult, SubscriptionQueryStatus)> GetVerifiedResult(string userId, string purchaseToken)
        {
            var result = await Verificator.VerifyAppleReceiptAsync(purchaseToken);

            if (result is null) return (null, SubscriptionQueryStatus.NotFound);

            return (result, await ValidateVerificationResult(userId, result));
        }

        async Task<SubscriptionQueryStatus> ValidateVerificationResult(string userId, AppleReceiptVerificationResult verificationResult)
        {
            if (verificationResult.AppleVerificationResponse is null)
            {
                Logger.LogWarning($"{verificationResult.Message}.");
                return SubscriptionQueryStatus.NotFound;
            }

            if (verificationResult.AppleVerificationResponse.StatusCode != IAPVerificationResponseStatus.Ok)
            {
                Logger.LogWarning($"{verificationResult.Message} [{verificationResult.AppleVerificationResponse.Status} - {verificationResult.AppleVerificationResponse.StatusCode}]");
                return SubscriptionQueryStatus.NotFound;
            }

            if (userId.IsEmpty()) return SubscriptionQueryStatus.Succeeded;

            var transactionIds = verificationResult.AppleVerificationResponse
                .LatestReceiptInfo
                .Select(x => x.TransactionId)
                .ToArray();
            if (transactionIds.None()) return SubscriptionQueryStatus.Succeeded;

            var originUserId = await Repository.GetOriginUserOfTransactionIds(transactionIds);
            if (userId?.Equals(originUserId, caseSensitive: false) ?? true) return SubscriptionQueryStatus.Succeeded;

            Logger.LogWarning($"This receipt is associated to {originUserId} and can't be used for {userId}.");
            return SubscriptionQueryStatus.UserMismatched;
        }
    }
}
