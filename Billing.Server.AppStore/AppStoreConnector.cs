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
    using AppStoreServerApi;
    using AppStoreServerApi.Models;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Olive;

    class AppStoreConnector : IStoreConnector
    {
        readonly ILogger<AppStoreConnector> Logger;
        readonly IAppleReceiptVerificatorService Verificator;
        readonly IOptionsSnapshot<AppleReceiptVerificationSettings> Settings;
        readonly AppStoreOptions Options;

        public AppStoreConnector(
            ILogger<AppStoreConnector> logger,
            IAppleReceiptVerificatorService verificator,
            IOptionsSnapshot<AppleReceiptVerificationSettings> settings,
            IOptions<AppStoreOptions> options
        )
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Verificator = verificator ?? throw new ArgumentNullException(nameof(verificator));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<SubscriptionInfo> GetSubscriptionInfo(SubscriptionInfoArgs args)
        {
            try
            {
                var result = await GetVerifiedResultV2(args.PackageName, args.OriginalTransactionId);
                if (result is null) return null;

                return CreateSubscriptionV2(result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to fetch a transaction history. TransationId: {TransationId}", args.OriginalTransactionId);

                var result = await GetVerifiedResult(args.PurchaseToken);
                if (result is null) return null;

                return CreateSubscription(result.AppleVerificationResponse);
            }
        }

        SubscriptionInfo CreateSubscription(IAPVerificationResponse response)
        {
            var purchase = response?.LatestReceiptInfo?.OrderBy(x => x.PurchaseDateDt).LastOrDefault();
            if (purchase is null)
            {
                Logger.LogWarning("The receipt contains no purchase info.");
                return null;
            }

            return new SubscriptionInfo
            {
                ProductId = purchase.ProductId,
                TransactionId = purchase.OriginalTransactionId,
                SubscriptionDate = purchase.PurchaseDateDt ?? DateTimeConverter.Convert(purchase.PurchaseDate),
                ExpirationDate = purchase.CancellationDateDt ?? DateTimeConverter.Convert(purchase.CancellationDate) ?? purchase.ExpirationDateDt ?? DateTimeConverter.Convert(purchase.ExpirationDate),
                CancellationDate = purchase.CancellationDateDt ?? DateTimeConverter.Convert(purchase.CancellationDate),
                AutoRenews = purchase.SubscriptionAutoRenewStatus == AppleSubscriptionAutoRenewStatus.Active
            };
        }

        async Task<AppleReceiptVerificationResult> GetVerifiedResult(string purchaseToken)
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

            if (result is null) return null;

            return ValidateVerificationResult(result);
        }

        async Task<AppleReceiptVerificationResult> GetLegacyResponse(string url, string purchaseToken)
        {
            var response = "(null)";

            try
            {
                var request = new IAPVerificationRequest(purchaseToken, Settings.Value.VerifyReceiptSharedSecret);
                response = await url.AsUri().PostJson(request);

                var notification = new AppStoreNotification
                {
                    UnifiedReceipt = JsonSerializer.Deserialize<AppStoreUnifiedReceipt>(response)
                };
                var result = JsonSerializer.Deserialize<IAPLegacyVerificationResult>(response);

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
                                SubscriptionAutoRenewStatus = result.AutoRenewStatus
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

        AppleReceiptVerificationResult ValidateVerificationResult(AppleReceiptVerificationResult verificationResult)
        {
            var response = verificationResult?.AppleVerificationResponse;

            if (response is null)
            {
                Logger.LogWarning($"{verificationResult.Message}.");
                return null;
            }

            if (response.StatusCode != IAPVerificationResponseStatus.Ok)
            {
                Logger.LogWarning($"{verificationResult.Message} [{response.Status} - {response.StatusCode}]");
                return null;
            }

            return verificationResult;
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

            [JsonPropertyName("auto_renew_status")]
            public AppleSubscriptionAutoRenewStatus AutoRenewStatus { get; set; }

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

        static SubscriptionInfo CreateSubscriptionV2(JWSTransactionDecodedPayload transaction) => new()
        {
            ProductId = transaction.ProductId,
            TransactionId = transaction.TransactionId,
            SubscriptionDate = transaction.PurchaseDate,
            ExpirationDate = transaction.ExpiresDate,
            CancellationDate = transaction.RevocationDate,
            AutoRenews = transaction.Type == InAppPurchaseProductType.AutoRenewableSubscription
        };

        async Task<JWSTransactionDecodedPayload?> GetVerifiedResultV2(string bundleId, string originalTransactionId)
        {
            try { return await Fetch(AppleEnvironment.Production, originalTransactionId); }
            catch { return await Fetch(AppleEnvironment.Sandbox, originalTransactionId); }

            async Task<JWSTransactionDecodedPayload?> Fetch(AppleEnvironment environment, string originalTransactionId)
            {
                var client = new AppStoreClient(environment, Options.PrivateKey, Options.KeyId, Options.IssuerId, bundleId);

                return await client.GetTransactionInfoAsync(originalTransactionId).Get(x => x.SignedTransactionInfo.DecodedPayload);
            }
        }
    }
}
