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

        public AppStoreConnector(ILogger<AppStoreConnector> logger, IAppleReceiptVerificatorService verificator)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Verificator = verificator ?? throw new ArgumentNullException(nameof(verificator));
        }

        public async Task<SubscriptionInfo> GetSubscriptionInfo(SubscriptionInfoArgs args)
        {
            var (result, status) = await GetVerifiedResult(args.PurchaseToken);

            return status switch
            {
                SubscriptionQueryStatus.NotFound => SubscriptionInfo.NotFound,
                SubscriptionQueryStatus.Expired => CreateExpiredSubscription(args.UserId, result.AppleVerificationResponse),
                _ => CreateSubscription(args.UserId, args.ProductId, result.AppleVerificationResponse)
            };
        }

        static SubscriptionInfo CreateExpiredSubscription(string userId, IAPVerificationResponse response)
        {
            return new SubscriptionInfo
            {
                UserId = userId,
                SubscriptionDate = response.Receipt.OriginalPurchaseDateDt,
                ExpirationDate = LocalTime.UtcNow
            };
        }

        SubscriptionInfo CreateSubscription(string userId, string productId, IAPVerificationResponse response)
        {
            var purchase = response?.LatestReceiptInfo.OrderBy(x => x.PurchaseDateDt).LastOrDefault(x => x.ProductId == productId);
            if (purchase is null)
            {
                Logger.LogError($"The receipt contains no purchase info for product id '{productId}'.");
                return SubscriptionInfo.NotFound;
            }

            return new SubscriptionInfo
            {
                UserId = userId,
                TransactionId = purchase.OriginalTransactionId,
                SubscriptionDate = purchase.PurchaseDateDt,
                ExpirationDate = purchase.ExpirationDateDt,
                CancellationDate = purchase.CancellationDateDt,
                AutoRenews = purchase.SubscriptionAutoRenewStatus == AppleSubscriptionAutoRenewStatus.Active
            };
        }

        async Task<(AppleReceiptVerificationResult, SubscriptionQueryStatus)> GetVerifiedResult(string purchaseToken)
        {
            var result = await Verificator.VerifyAppleReceiptAsync(purchaseToken);

            if (result?.Status == IAPVerificationResponseStatus.TestReceiptOnProd)
                result = await Verificator.VerifyAppleSandBoxReceiptAsync(purchaseToken);
            else if (result?.Status == IAPVerificationResponseStatus.ProdReceiptOnTest)
                result = await Verificator.VerifyAppleProductionReceiptAsync(purchaseToken);

            if (result is null) return (null, SubscriptionQueryStatus.NotFound);

            return (result, ValidateVerificationResult(result));
        }

        SubscriptionQueryStatus ValidateVerificationResult(AppleReceiptVerificationResult verificationResult)
        {
            var response = verificationResult?.AppleVerificationResponse;

            if (response is null)
            {
                Logger.LogWarning($"{verificationResult.Message}.");
                return SubscriptionQueryStatus.NotFound;
            }

            if (response.StatusCode != IAPVerificationResponseStatus.Ok)
            {
                Logger.LogWarning($"{verificationResult.Message} [{response.Status} - {response.StatusCode}]");
                if (response.StatusCode == IAPVerificationResponseStatus.SubscriptionExpired) return SubscriptionQueryStatus.Expired;
                return SubscriptionQueryStatus.NotFound;
            }

            return SubscriptionQueryStatus.Succeeded;
        }
    }
}
