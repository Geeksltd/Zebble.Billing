namespace Zebble.Billing
{
	using System;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Threading.Tasks;
	using Apple.Receipt.Models;
	using Apple.Receipt.Models.Enums;
	using Apple.Receipt.Verificator.Models;
	using Apple.Receipt.Verificator.Models.IAPVerification;
	using Apple.Receipt.Verificator.Services;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using Newtonsoft.Json;
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
			var purchase = response?.LatestReceiptInfo?.OrderBy(x => x.PurchaseDateDt).LastOrDefault(x => x.ProductId == productId);
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
				response = await url.AsUri().PostJson(request);
				var result = JsonConvert.DeserializeObject<IAPLegacyVerificationResult>(response);

				var verificationResult = new AppleReceiptVerificationResult(null, result.Status);

				if (result.Receipt is not null)
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
							}
						}
					};
				};

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

			[JsonProperty(PropertyName = "receipt-data")]
			public string ReceiptData { get; set; }

			[JsonProperty(PropertyName = "password")]
			public string Password { get; set; }
		}

		class IAPLegacyVerificationResult
		{
			[JsonProperty(PropertyName = "receipt")]
			public IAPLegacyReceipt Receipt { get; set; }

			[JsonProperty(PropertyName = "status")]
			public IAPVerificationResponseStatus Status { get; set; }
		}

		class IAPLegacyReceipt
		{
			[JsonProperty("expires_date_ms")]
			public string ExpiresDateMs { get; set; }

			[JsonProperty("purchase_date_ms")]
			public string PurchaseDateMs { get; set; }

			[JsonProperty("original_purchase_date_ms")]
			public string OriginalPurchaseDateMs { get; set; }

			[JsonProperty("original_transaction_id")]
			public string OriginalTransactionId { get; set; }

			[JsonProperty("product_id")]
			public string ProductId { get; set; }
		}
	}
}
