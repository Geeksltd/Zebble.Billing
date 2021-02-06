namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;

    class AppStoreHookInterceptor
    {
        readonly ISubscriptionRepository Repository;

        public AppStoreHookInterceptor(ISubscriptionRepository repository)
        {
            Repository = repository;
        }

        public async Task Intercept(string body)
        {
            var notification = body.ToNotification();

            await Repository.AddTransaction(new Transaction
            {
                TransactionId = Guid.NewGuid().ToString(),
                //SubscriptionId = subscription.SubscriptionId,
                Platform = "AppStore",
                //Date = notification.EventTime,
                //Details = notification.OriginalData
            });
        }
    }
}
