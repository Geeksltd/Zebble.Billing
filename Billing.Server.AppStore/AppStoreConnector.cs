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
        readonly IAppleReceiptVerificatorService ReceiptVerificator;

        public AppStoreConnector(IAppleReceiptVerificatorService receiptVerificator)
        {
            ReceiptVerificator = receiptVerificator ?? throw new ArgumentNullException(nameof(receiptVerificator));
        }

        public async Task<bool> VerifyPurchase(VerifyPurchaseArgs args)
        {
            var result = await ReceiptVerificator.VerifyAppleReceiptAsync(args.ReceiptData);

            if (result == null) return false;

            ValidateVerificationResult(result);

            return true;
        }

        public async Task<SubscriptionInfo> GetSubscriptionInfo(SubscriptionInfoArgs args)
        {
            var result = await ReceiptVerificator.VerifyAppleReceiptAsync(args.ReceiptData);

            if (result == null) return null;

            ValidateVerificationResult(result);

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

        void ValidateVerificationResult(AppleReceiptVerificationResult verificationResult)
        {
            if (verificationResult.AppleVerificationResponse == null)
                throw new Exception($"{verificationResult.Message}.");

            if (verificationResult.AppleVerificationResponse.StatusCode != IAPVerificationResponseStatus.Ok)
                throw new Exception($"{verificationResult.Message} [{verificationResult.AppleVerificationResponse.Status} - {verificationResult.AppleVerificationResponse.StatusCode}]");
        }
    }
}
