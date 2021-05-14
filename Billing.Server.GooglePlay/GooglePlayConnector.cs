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

            try
            {
                if (product.Type == ProductType.Subscription)
                {
                    var subscriptionResult = await Publisher.Purchases.Subscriptions.Get(Options.PackageName, args.ProductId, args.PurchaseToken).ExecuteAsync();

                    if (subscriptionResult is null) return SubscriptionInfo.NotFound;
                    return CreateSubscription(args.UserId, subscriptionResult);
                }

                var productResult = await Publisher.Purchases.Products.Get(Options.PackageName, args.ProductId, args.PurchaseToken).ExecuteAsync();

                if (productResult is null) return SubscriptionInfo.NotFound;
                return CreateSubscription(args.UserId, productResult);
            }
            catch (GoogleApiException ex)
            {
                if (ex.Error.Errors.Any(x => x.Reason.IsAnyOf("subscriptionPurchaseNoLongerAvailable")))
                    return new SubscriptionInfo
                    {
                        UserId = args.UserId,
                        ExpirationDate = LocalTime.UtcToday
                    };

                if (ex.Error.Errors.Any(x => x.Reason.IsAnyOf("purchaseTokenNoLongerValid")))
                    return new SubscriptionInfo
                    {
                        UserId = args.UserId,
                        CancellationDate = LocalTime.UtcToday
                    };

                throw;
            }
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
    }
}
