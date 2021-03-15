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
            return await GetVerifiedResult(args.ReceiptData) is not null;
        }

        public async Task<SubscriptionInfo> GetSubscriptionInfo(SubscriptionInfoArgs args)
        {
            var result = await GetVerifiedResult(args.ReceiptData);

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

        async Task<AppleReceiptVerificationResult> GetVerifiedResult(string receiptData)
        {
            var result = await ReceiptVerificator.VerifyAppleReceiptAsync(receiptData);

            if (result is not null) ValidateVerificationResult(result);

            return result;
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
