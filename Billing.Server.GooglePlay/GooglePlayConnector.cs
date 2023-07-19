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
                var subscriptionResult = await Execute(x => x.Subscriptionsv2.Get(Options.PackageName, args.PurchaseToken));
                if (subscriptionResult is null) return null;

                return CreateSubscription(subscriptionResult);
            }

            var productResult = await Execute(x => x.Products.Get(Options.PackageName, args.ProductId, args.PurchaseToken));
            if (productResult is null) return null;

            return CreateSubscription(productResult);
        }

        static SubscriptionInfo CreateSubscription(SubscriptionPurchaseV2 purchase)
        {
            var lineItem = purchase.LineItems.OrderBy(x => x.ExpiryTime).LastOrDefault();
            if (lineItem is null) return null;

            return new SubscriptionInfo
            {
                ProductId = lineItem.ProductId,
                TransactionId = purchase.LatestOrderId,
                SubscriptionDate = DateTimeOffset.Parse(purchase.StartTime.ToString()).DateTime,
                ExpirationDate = DateTimeOffset.Parse(lineItem.ExpiryTime.ToString()).DateTime,
                CancellationDate = DateTimeOffset.Parse(purchase.CanceledStateContext.UserInitiatedCancellation.CancelTime.ToString()).DateTime,
                AutoRenews = lineItem.AutoRenewingPlan.AutoRenewEnabled ?? false
            };
        }

        static SubscriptionInfo CreateSubscription(ProductPurchase purchase)
        {
            return new SubscriptionInfo
            {
                ProductId = purchase.ProductId,
                TransactionId = purchase.OrderId,
                SubscriptionDate = purchase.PurchaseTimeMillis.ToDateTime(),
                CancellationDate = purchase.PurchaseState == 1 ? LocalTime.UtcNow : null
            };
        }

        async Task<TResult> Execute<TResult>(Func<PurchasesResource, AndroidPublisherBaseServiceRequest<TResult>> callee)
        {
            try
            {
                var result = await callee(Publisher.Purchases).ExecuteAsync();
                return result;
            }
            catch (GoogleApiException ex)
            {
                var reasons = ex.Error.Errors.Select(x => x.Reason).ToArray();
                if (reasons.ContainsAny("subscriptionPurchaseNoLongerAvailable", "purchaseTokenNoLongerValid"))
                    return default;

                throw;
            }
        }
    }
}
