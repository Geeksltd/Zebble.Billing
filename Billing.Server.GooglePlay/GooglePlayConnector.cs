namespace Zebble.Billing
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Google.Apis.AndroidPublisher.v3;
    using Google.Apis.AndroidPublisher.v3.Data;
    using Olive;
    using System;
    using Google;

    class GooglePlayConnector : IStoreConnector
    {
        readonly GooglePlayOptions Options;
        readonly IProductProvider Provider;
        readonly AndroidPublisherService Publisher;

        public GooglePlayConnector(IOptionsSnapshot<GooglePlayOptions> options, IProductProvider provider, AndroidPublisherService publisher)
        {
            Options = options.Value ?? throw new ArgumentNullException(nameof(options));
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
            Publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        }

        public async Task<SubscriptionInfo> GetSubscriptionInfo(SubscriptionInfoArgs args)
        {
            var product = await Provider.GetById(args.ProductId);

            if (product.Type == ProductType.Subscription)
            {
                var (subscriptionResult, subscriptionStatus) = await Execute(x => x.Subscriptions.Get(Options.PackageName, args.ProductId, args.PurchaseToken));

                return subscriptionStatus switch
                {
                    SubscriptionQueryStatus.NotFound => SubscriptionInfo.NotFound,
                    SubscriptionQueryStatus.Expired => CreateExpiredSubscription(args.UserId),
                    _ => CreateSubscription(args.UserId, subscriptionResult)
                };
            }

            var (productResult, productStatus) = await Execute(x => x.Products.Get(Options.PackageName, args.ProductId, args.PurchaseToken));

            return productStatus switch
            {
                SubscriptionQueryStatus.NotFound => SubscriptionInfo.NotFound,
                SubscriptionQueryStatus.Expired => CreateExpiredSubscription(args.UserId),
                _ => CreateSubscription(args.UserId, productResult)
            };
        }

        SubscriptionInfo CreateExpiredSubscription(string userId)
        {
            return new SubscriptionInfo
            {
                UserId = userId,
                SubscriptionDate = LocalTime.UtcToday,
                ExpirationDate = LocalTime.UtcNow
            };
        }

        SubscriptionInfo CreateSubscription(string userId, SubscriptionPurchase purchase)
        {
            return new SubscriptionInfo
            {
                UserId = userId.Or(purchase.DeveloperPayload).Or(purchase.EmailAddress),
                TransactionId = purchase.OrderId,
                SubscriptionDate = purchase.StartTimeMillis.ToDateTime(),
                ExpirationDate = purchase.ExpiryTimeMillis.ToDateTime(),
                CancellationDate = purchase.UserCancellationTimeMillis.ToDateTime(),
                AutoRenews = purchase.AutoRenewing ?? false
            };
        }

        SubscriptionInfo CreateSubscription(string userId, ProductPurchase purchase)
        {
            return new SubscriptionInfo
            {
                UserId = userId.Or(purchase.DeveloperPayload),
                TransactionId = purchase.OrderId,
                SubscriptionDate = purchase.PurchaseTimeMillis.ToDateTime(),
                CancellationDate = purchase.PurchaseState == 1 ? LocalTime.UtcNow : null
            };
        }

        async Task<(TResult, SubscriptionQueryStatus)> Execute<TResult>(Func<PurchasesResource, AndroidPublisherBaseServiceRequest<TResult>> callee)
        {
            try
            {
                var result = await callee(Publisher.Purchases).ExecuteAsync();

                if (result is null) return (default, SubscriptionQueryStatus.NotFound);
                return (result, SubscriptionQueryStatus.Succeeded);
            }
            catch (GoogleApiException ex)
            {
                var reasons = ex.Error.Errors.Select(x => x.Reason).ToArray();
                if (reasons.ContainsAny("subscriptionPurchaseNoLongerAvailable", "purchaseTokenNoLongerValid"))
                    return (default, SubscriptionQueryStatus.Expired);

                throw;
            }
        }
    }
}
