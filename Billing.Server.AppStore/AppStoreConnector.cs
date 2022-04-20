namespace Zebble.Billing
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using Apple.Receipt.Models;
    using Apple.Receipt.Models.Enums;
    using Apple.Receipt.Verificator.Models;
    using Apple.Receipt.Verificator.Models.IAPVerification;
    using Apple.Receipt.Verificator.Services;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Olive;

    class AppStoreConnector : IStoreConnector
    {
        readonly ILogger<AppStoreConnector> Logger;
        readonly IAppleReceiptVerificatorService Verificator;
        readonly IOptionsSnapshot<AppleReceiptVerificationSettings> Settings;

        public AppStoreConnector(
            ILogger<AppStoreConnector> logger,
            IAppleReceiptVerificatorService verificator,
            IOptionsSnapshot<AppleReceiptVerificationSettings> settings
        )
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Verificator = verificator ?? throw new ArgumentNullException(nameof(verificator));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<SubscriptionInfo> GetSubscriptionInfo(SubscriptionInfoArgs args)
        {
            var (result, status) = await GetVerifiedResult(args.PurchaseToken);

            return status switch
            {
                SubscriptionQueryStatus.NotFound => SubscriptionInfo.NotFound,
                _ => CreateSubscription(args.UserId, args.ProductId, result.AppleVerificationResponse)
            };
        }

        SubscriptionInfo CreateSubscription(string userId, string productId, IAPVerificationResponse response)
        {
            var purchase = response?.LatestReceiptInfo?.OrderBy(x => x.PurchaseDateDt).LastOrDefault(x => x.ProductId == productId);
            if (purchase is null)
            {
                Logger.LogWarning($"The receipt contains no purchase info for product id '{productId}'.");
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

            if (result?.AppleVerificationResponse?.LatestReceiptInfo == null)
            {
                result = await GetLegacyResponse(Settings.Value.VerifyUrl, purchaseToken);

                if (result?.Status == IAPVerificationResponseStatus.TestReceiptOnProd)
                    result = await GetLegacyResponse(Settings.Value.SandboxUrl, purchaseToken);
                else if (result?.Status == IAPVerificationResponseStatus.ProdReceiptOnTest)
                    result = await GetLegacyResponse(Settings.Value.ProductionUrl, purchaseToken);
            }

            if (result is null) return (null, SubscriptionQueryStatus.NotFound);

            return (result, ValidateVerificationResult(result));
        }

        async Task<AppleReceiptVerificationResult> GetLegacyResponse(string url, string purchaseToken)
        {
            var response = "(null)";

            try
            {
                var request = new IAPVerificationRequest(purchaseToken, Settings.Value.VerifyReceiptSharedSecret);
                var rawResponse = await url.AsUri().PostJson(request);

                var notification = new AppStoreNotification
                {
                    UnifiedReceipt = JsonSerializer.Deserialize<AppStoreUnifiedReceipt>(rawResponse)
                };
                var result = JsonSerializer.Deserialize<IAPLegacyVerificationResult>(rawResponse);

                var verificationResult = new AppleReceiptVerificationResult(null, result.Status);

                if (notification.ProductId.HasValue())
                {
                    verificationResult.AppleVerificationResponse = new IAPVerificationResponse
                    {
                        Receipt = new AppleAppReceipt
                        {
                            OriginalPurchaseDateMs = notification.OriginalPurchaseDateMs,
                        },
                        LatestReceiptInfo = new Collection<AppleInAppPurchaseReceipt>
                        {
                            new AppleInAppPurchaseReceipt {
                                PurchaseDateMs = notification.PurchaseDateMs,
                                ProductId = notification.ProductId,
                                OriginalTransactionId = notification.OriginalTransactionId,
                                ExpirationDateMs = notification.ExpiresDateMs,
                                SubscriptionAutoRenewStatus = notification.AutoRenewStatus == true ?
                                    AppleSubscriptionAutoRenewStatus.Active :
                                    AppleSubscriptionAutoRenewStatus.Disabled
                            }
                        }
                    };
                }
                else if (result.Receipt is not null)
                {
                    verificationResult.AppleVerificationResponse = new IAPVerificationResponse
                    {
                        Receipt = new AppleAppReceipt
                        {
                            OriginalPurchaseDateMs = result.Receipt.OriginalPurchaseDateMs,
                        },
                        LatestReceiptInfo = new Collection<AppleInAppPurchaseReceipt>
                        {
                            new AppleInAppPurchaseReceipt {
                                PurchaseDateMs = result.Receipt.PurchaseDateMs,
                                ProductId = result.Receipt.ProductId,
                                OriginalTransactionId = result.Receipt.OriginalTransactionId,
                                ExpirationDateMs = result.Receipt.ExpiresDateMs,
                                SubscriptionAutoRenewStatus = notification.AutoRenewStatus == true ?
                                    AppleSubscriptionAutoRenewStatus.Active :
                                    AppleSubscriptionAutoRenewStatus.Disabled
                            }
                        }
                    };
                }

                return verificationResult;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Failed to verify legacy receipt. Url: {url}, Response: {response}");
                throw;
            }
        }

        SubscriptionQueryStatus ValidateVerificationResult(AppleReceiptVerificationResult verificationResult)
        {
            if (verificationResult?.Status == IAPVerificationResponseStatus.SubscriptionExpired)
                return SubscriptionQueryStatus.Expired;

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

        class IAPVerificationRequest
        {
            public IAPVerificationRequest(string receiptData, string password)
            {
                ReceiptData = receiptData;
                Password = password;
            }

            [JsonPropertyName("receipt-data")]
            public string ReceiptData { get; set; }

            [JsonPropertyName("password")]
            public string Password { get; set; }
        }

        class IAPLegacyVerificationResult
        {
            [JsonPropertyName("receipt")]
            public IAPLegacyReceipt Receipt { get; set; }

            [JsonPropertyName("status")]
            public IAPVerificationResponseStatus Status { get; set; }
        }

        class IAPLegacyReceipt
        {
            [JsonPropertyName("expires_date")]
            public string ExpiresDateMs { get; set; }

            [JsonPropertyName("purchase_date_ms")]
            public string PurchaseDateMs { get; set; }

            [JsonPropertyName("original_purchase_date_ms")]
            public string OriginalPurchaseDateMs { get; set; }

            [JsonPropertyName("original_transaction_id")]
            public string OriginalTransactionId { get; set; }

            [JsonPropertyName("product_id")]
            public string ProductId { get; set; }
        }
    }
}
