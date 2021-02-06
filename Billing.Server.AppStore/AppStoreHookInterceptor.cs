namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;

    class AppStoreHookInterceptor
    {
        private readonly ISubscriptionRepository repository;

        public AppStoreHookInterceptor(ISubscriptionRepository repository)
        {
            this.repository = repository;
        }

        public async Task Intercept(string body)
        {
            var notification = body.ToNotification();

            await repository.AddTransaction(new Transaction
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
