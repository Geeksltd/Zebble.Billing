﻿namespace Zebble.Billing
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
        readonly IAppleReceiptVerificatorService ReceiptVerificator;
        readonly ISubscriptionRepository Repository;

        public AppStoreConnector(ILogger<AppStoreConnector> logger, IAppleReceiptVerificatorService receiptVerificator, ISubscriptionRepository repository)
        {
            Logger = logger;
            ReceiptVerificator = receiptVerificator ?? throw new ArgumentNullException(nameof(receiptVerificator));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<PurchaseVerificationResult> VerifyPurchase(VerifyPurchaseArgs args)
        {
            var (_, status) = await GetVerifiedResult(args.UserId, args.ReceiptData);

            return status;
        }

        public async Task<SubscriptionInfo> GetSubscriptionInfo(SubscriptionInfoArgs args)
        {
            var (result, _) = await GetVerifiedResult(args.UserId, args.ReceiptData);

            return CreateSubscription(result.AppleVerificationResponse.LatestReceiptInfo.First());
        }

        SubscriptionInfo CreateSubscription(AppleInAppPurchaseReceipt purchase)
        {
            return new SubscriptionInfo
            {
                UserId = null,
                TransactionId = purchase.OriginalTransactionId,
                SubscriptionDate = purchase.PurchaseDateDt,
                ExpirationDate = purchase.ExpirationDateDt,
                CancellationDate = purchase.CancellationDateDt,
                AutoRenews = purchase.SubscriptionAutoRenewStatus == AppleSubscriptionAutoRenewStatus.Active
            };
        }

        async Task<(AppleReceiptVerificationResult, PurchaseVerificationResult)> GetVerifiedResult(string userId, string receiptData)
        {
            var result = await ReceiptVerificator.VerifyAppleReceiptAsync(receiptData);

            if (result is null) return (null, PurchaseVerificationResult.Failed);

            return (result, await ValidateVerificationResult(userId, result));
        }

        async Task<PurchaseVerificationResult> ValidateVerificationResult(string userId, AppleReceiptVerificationResult verificationResult)
        {
            if (verificationResult.AppleVerificationResponse == null)
            {
                Logger.LogWarning($"{verificationResult.Message}.");
                return PurchaseVerificationResult.Failed;
            }

            if (verificationResult.AppleVerificationResponse.StatusCode != IAPVerificationResponseStatus.Ok)
            {
                Logger.LogWarning($"{verificationResult.Message} [{verificationResult.AppleVerificationResponse.Status} - {verificationResult.AppleVerificationResponse.StatusCode}]");
                return PurchaseVerificationResult.Failed;
            }

            if (userId.IsEmpty()) return PurchaseVerificationResult.Verified;

            var transactionIds = verificationResult.AppleVerificationResponse
                .LatestReceiptInfo
                .Select(x => x.TransactionId)
                .ToArray();
            if (transactionIds.None()) return PurchaseVerificationResult.Verified;

            var originUserId = await Repository.GetOriginUserOfTransactionIds(transactionIds);
            if (originUserId.IsEmpty()) return PurchaseVerificationResult.Verified;

            if (!userId.Equals(originUserId, caseSensitive: false))
            {
                Logger.LogWarning($"This receipt is associated to {originUserId} and can't be used for {userId}.");
                return PurchaseVerificationResult.UserMismatched;
            }

            return PurchaseVerificationResult.Verified;
        }
    }
}
