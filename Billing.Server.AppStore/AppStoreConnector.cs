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
    using Olive;

    class AppStoreConnector : IStoreConnector
    {
        readonly IAppleReceiptVerificatorService AppleReceiptVerificator;

        public AppStoreConnector(IAppleReceiptVerificatorService appleReceiptVerificator)
        {
            AppleReceiptVerificator = appleReceiptVerificator;
        }

        public async Task<Subscription> GetUpToDateInfo(string productId, string purchaseToken)
        {
            var result = await AppleReceiptVerificator.VerifyAppleReceiptAsync(purchaseToken);

            if (result == null)
                return null;

            ValidateVerificationResult(result);

            return CreateSubscription(productId, purchaseToken, result.AppleVerificationResponse.LatestReceiptInfo.First());
        }

        Subscription CreateSubscription(string productId, string purchaseToken, AppleInAppPurchaseReceipt purchase)
        {
            return new Subscription
            {
                SubscriptionId = Guid.NewGuid().ToString(),
                ProductId = productId,
                UserId = null,
                Platform = "AppStore",
                PurchaseToken = purchaseToken,
                OriginalTransactionId = purchase.OriginalTransactionId,
                DateSubscribed = purchase.PurchaseDateDt,
                ExpiryDate = purchase.ExpirationDateDt,
                CancellationDate = purchase.CancellationDateDt,
                LastUpdated = LocalTime.Now,
                AutoRenews = purchase.SubscriptionAutoRenewStatus == AppleSubscriptionAutoRenewStatus.Active
            };
        }

        void ValidateVerificationResult(AppleReceiptVerificationResult verificationResult)
        {
            if (verificationResult.AppleVerificationResponse.StatusCode != IAPVerificationResponseStatus.Ok)
                throw new Exception($"{verificationResult.Message} ({verificationResult.AppleVerificationResponse.StatusCode} [{verificationResult.AppleVerificationResponse.Status}])");
        }
    }
}
