namespace Zebble.Billing
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using Google.Cloud.PubSub.V1;
	using Olive;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Logging;

	class GooglePlayQueueProcessor
	{
		readonly ILogger<GooglePlayQueueProcessor> Logger;
		readonly IServiceProvider Services;
		readonly GooglePlayConnector StoreConnector;
		readonly SubscriberClient Subscriber;

		public GooglePlayQueueProcessor(
			ILogger<GooglePlayQueueProcessor> logger,
			IServiceProvider services,
			GooglePlayConnector storeConnector,
			SubscriberClient subscriber
		)
		{
			Logger = logger;
			Services = services;
			StoreConnector = storeConnector;
			Subscriber = subscriber;
		}

		public async Task<int> Process()
		{
			var messageCount = 0;

			var startTask = Subscriber.StartAsync(async (message, _) =>
			{
				var notification = message.ToNotification();

				Interlocked.Increment(ref messageCount);

				if (!notification.IsTest) await ProccessNotification(notification);

				return SubscriberClient.Reply.Ack;
			});

			await Task.Delay(5.Seconds());
			await Subscriber.StopAsync(CancellationToken.None);

			await startTask;

			Logger.Debug($"{messageCount} queue messages are processed.");

			return messageCount;
		}

		async Task ProccessNotification(GooglePlayNotification notification)
		{
			try
			{
				using var scope = Services.CreateScope();
				var repository = scope.ServiceProvider.GetRequiredService<ISubscriptionRepository>();

				var subscription = await repository.GetByPurchaseToken(notification.PurchaseToken);

				if (subscription is null)
				{
					var subscriptionInfo = await StoreConnector.GetSubscriptionInfo(notification.ToArgs());
					if (subscriptionInfo.Status != SubscriptionQueryStatus.Succeeded) return;

					var oldSubscription = await repository.GetByTransactionId(subscriptionInfo.TransactionId);

					subscription = await repository.AddSubscription(new Subscription
					{
						Id = Guid.NewGuid().ToString(),
						ProductId = notification.ProductId,
						UserId = subscriptionInfo.UserId.Or(oldSubscription?.UserId).Or("<NOT_PROVIDED>"),
						Platform = "GooglePlay",
						TransactionId = subscriptionInfo.TransactionId,
						TransactionDate = notification.EventTime,
						PurchaseToken = notification.PurchaseToken,
						SubscriptionDate = subscriptionInfo.SubscriptionDate,
						ExpirationDate = subscriptionInfo.ExpirationDate,
						CancellationDate = subscriptionInfo.CancellationDate,
						LastUpdate = LocalTime.UtcNow,
						AutoRenews = subscriptionInfo.AutoRenews
					});
				}

				if (notification.State.IsAnyOf(GooglePlaySubscriptionState.Purchased, GooglePlaySubscriptionState.Renewed))
					subscription.SubscriptionDate = notification.EventTime;
				else if (notification.State == GooglePlaySubscriptionState.Canceled)
					subscription.CancellationDate = notification.EventTime;
				else if (notification.State.IsAnyOf(GooglePlaySubscriptionState.Expired, GooglePlaySubscriptionState.Revoked))
					subscription.ExpirationDate = notification.EventTime;

				await repository.UpdateSubscription(subscription);

				await repository.AddTransaction(new Transaction
				{
					Id = Guid.NewGuid().ToString(),
					SubscriptionId = subscription.Id,
					Platform = "GooglePlay",
					Date = notification.EventTime ?? LocalTime.UtcNow,
					Details = notification.OriginalData
				});
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, $"Failed to process following notification. {notification.OriginalData}");
			}
		}
	}
}
