namespace Zebble.Billing;

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
    readonly ISubscriptionChangeHandler SubscriptionChangeHandler;

    public AppStoreHookInterceptor(
        ILogger<AppStoreHookInterceptor> logger,
        IOptionsSnapshot<AppStoreOptions> options,
        ISubscriptionRepository repository,
        AppStoreConnector storeConnector,
        ISubscriptionChangeHandler subscriptionChangeHandler
    )
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        Options = options.Value ?? throw new ArgumentNullException(nameof(options));
        Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        StoreConnector = storeConnector ?? throw new ArgumentNullException(nameof(storeConnector));
        SubscriptionChangeHandler = subscriptionChangeHandler ?? throw new ArgumentNullException(nameof(subscriptionChangeHandler));
    }

    public async Task Intercept(AppStoreDecodedNotification notification)
    {
        try
        {
            ValidateNotification(notification);

            var subscriptionInfo = await StoreConnector.GetSubscriptionInfo(notification.ToArgs());
            if (subscriptionInfo is null) return;

            var subscription = await Repository.GetWithTransactionId(subscriptionInfo.TransactionId);

            if (subscription is not null)
            {
                subscription.ProductId = (notification.Data?.SignedRenewalInfo?.AutoRenewProductId).Or(subscriptionInfo.ProductId);
                subscription.TransactionDate = subscriptionInfo.SubscriptionDate;
                subscription.SubscriptionDate = subscriptionInfo.SubscriptionDate;
                subscription.ExpirationDate = subscriptionInfo.ExpirationDate;
                subscription.CancellationDate = subscriptionInfo.CancellationDate;
                subscription.LastUpdate = LocalTime.UtcNow;
                subscription.AutoRenews = subscriptionInfo.AutoRenews;

                await Repository.UpdateSubscription(subscription);

                await SubscriptionChangeHandler.Handle(subscription);
            }

            await Repository.AddTransaction(new Transaction
            {
                Id = Guid.NewGuid().ToString(),
                SubscriptionId = subscription?.Id,
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

    void ValidateNotification(AppStoreDecodedNotification notification)
    {
        if (Options.AllowEnvironmentMixing) return;
        if (notification.Data.Environment != Options.Environment) throw new Exception("Environment doesn't match.");
    }
}
