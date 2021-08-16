namespace Zebble.Billing
{
	using System;
	using System.Threading.Tasks;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using Olive;

	class AppStoreHookInterceptor
	{
		readonly ILogger<AppStoreHookInterceptor> Logger;
		readonly AppStoreOptions Options;
		readonly ISubscriptionRepository Repository;
		readonly AppStoreConnector StoreConnector;

		public AppStoreHookInterceptor(
			ILogger<AppStoreHookInterceptor> logger,
			IOptionsSnapshot<AppStoreOptions> options,
			ISubscriptionRepository repository,
			AppStoreConnector storeConnector
		)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			Options = options.Value ?? throw new ArgumentNullException(nameof(options));
			Repository = repository ?? throw new ArgumentNullException(nameof(repository));
			StoreConnector = storeConnector ?? throw new ArgumentNullException(nameof(storeConnector));
		}

		public async Task Intercept(AppStoreNotification notification)
		{
			try
			{
				ValidateNotification(notification);

				var subscription = await Repository.GetByPurchaseToken(notification.PurchaseToken);

				if (subscription is null)
				{
					var subscriptionInfo = await StoreConnector.GetSubscriptionInfo(notification.ToArgs());
					if (subscriptionInfo.Status != SubscriptionQueryStatus.Succeeded) return;

					var oldSubscription = await Repository.GetByTransactionId(subscriptionInfo.TransactionId);

					subscription = await Repository.AddSubscription(new Subscription
					{
						Id = Guid.NewGuid().ToString(),
						ProductId = notification.ProductId,
						UserId = subscriptionInfo.UserId.Or(oldSubscription?.UserId).Or("<NOT_PROVIDED>"),
						Platform = "AppStore",
						TransactionId = subscriptionInfo.TransactionId,
						PurchaseToken = notification.PurchaseToken,
						TransactionDate = notification.PurchaseDate,
						SubscriptionDate = subscriptionInfo.SubscriptionDate,
						ExpirationDate = subscriptionInfo.ExpirationDate,
						CancellationDate = subscriptionInfo.CancellationDate,
						LastUpdate = LocalTime.UtcNow,
						AutoRenews = subscriptionInfo.AutoRenews
					});
				}

				if (notification.Type.IsAnyOf(AppStoreNotificationType.InitialBuy, AppStoreNotificationType.InteractivelyRenewed))
				{
					subscription.SubscriptionDate = notification.PurchaseDate;
					subscription.ExpirationDate = notification.ExpirationDate;
					subscription.CancellationDate = notification.CancellationDate;
					subscription.AutoRenews = notification.AutoRenewStatus;
				}
				else if (notification.Type.IsAnyOf(AppStoreNotificationType.CanceledOrUpgraded, AppStoreNotificationType.Refunded, AppStoreNotificationType.FamilySharingRevoked))
					subscription.CancellationDate = notification.CancellationDate;
				else if (notification.Type.IsAnyOf(AppStoreNotificationType.AutoRenewed, AppStoreNotificationType.AutoRecovered))
					subscription.ExpirationDate = notification.ExpirationDate;
				else if (notification.Type == AppStoreNotificationType.AutoRenewFailed)
					subscription.ExpirationDate = notification.IsInBillingRetryPeriod == true ? notification.GracePeriodExpirationDate : LocalTime.UtcNow;
				else if (notification.Type == AppStoreNotificationType.RenewalStatusChanged)
					subscription.AutoRenews = notification.AutoRenewStatus;

				subscription.LastUpdate = LocalTime.UtcNow;

				await Repository.UpdateSubscription(subscription);

				await Repository.AddTransaction(new Transaction
				{
					Id = Guid.NewGuid().ToString(),
					SubscriptionId = subscription.Id,
					Platform = "AppStore",
					Date = LocalTime.UtcNow,
					Details = notification.OriginalData
				});
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, $"Failed to intercept the following notification. {notification.OriginalData}");
				throw;
			}
		}

		void ValidateNotification(AppStoreNotification notification)
		{
			if (notification.SharedSecret != Options.SharedSecret) throw new Exception("SharedSecret doesn't match.");

			if (notification.Environment != Options.Environment) throw new Exception("Environment doesn't match.");
		}
	}
}
