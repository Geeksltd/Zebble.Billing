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
        readonly AndroidPublisherService Publisher;

        public GooglePlayConnector(IOptionsSnapshot<GooglePlayOptions> options, AndroidPublisherService publisher)
        {
            Options = options.Value ?? throw new ArgumentNullException(nameof(options));
            Publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        }

        public async Task<SubscriptionInfo> GetSubscriptionInfo(SubscriptionInfoArgs args)
        {
            try
            {
                var subscriptionResult = await Execute(x => x.Subscriptionsv2.Get(args.PackageName, args.PurchaseToken));

                if (subscriptionResult is null) return null;
                return CreateSubscription(subscriptionResult);
            }
            catch (GoogleApiException)
            {
                var productResult = await Execute(x => x.Products.Get(args.PackageName, args.ProductId, args.PurchaseToken));

                if (productResult is null) return null;
                return CreateSubscription(productResult, args.ProductId);
            }
        }

        static SubscriptionInfo CreateSubscription(SubscriptionPurchaseV2 purchase)
        {
            var lineItem = purchase.LineItems.OrderBy(x => x.ExpiryTime).LastOrDefault();
            if (lineItem is null) return null;

            static DateTime? ToDataTime(object value)
            {
                if (value is null) return null;
                try { return DateTimeOffset.Parse(value?.ToString()).DateTime; }
                catch { return null; }
            }

            return new SubscriptionInfo
            {
                ProductId = lineItem.ProductId,
                TransactionId = purchase.LatestOrderId,
                SubscriptionDate = ToDataTime(purchase.StartTime),
                ExpirationDate = ToDataTime(lineItem.ExpiryTime),
                CancellationDate = ToDataTime(purchase.CanceledStateContext?.UserInitiatedCancellation?.CancelTime),
                AutoRenews = lineItem.AutoRenewingPlan?.AutoRenewEnabled ?? false
            };
        }

        static SubscriptionInfo CreateSubscription(ProductPurchase purchase, string productId)
        {
            return new SubscriptionInfo
            {
                ProductId = purchase.ProductId.Or(productId),
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
                if (reasons.ContainsAny("subscriptionPurchaseNoLongerAvailable", "purchaseTokenNoLongerValid", "invalid"))
                    return default;

                var messages = ex.Error.Errors.Select(x => x.Message).ToArray();
                if (messages.Any(msg => msg.Contains("expired", caseSensitive: false)))
                    return default;

                throw;
            }
        }
    }
}
