namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Apple.Receipt.Verificator.Services;

    class AppStoreHookInterceptor
    {
        readonly ISubscriptionRepository Repository;
        readonly IAppleReceiptVerificatorService AppleReceiptVerificator;

        public AppStoreHookInterceptor(ISubscriptionRepository repository, IAppleReceiptVerificatorService appleReceiptVerificator)
        {
            Repository = repository;
            AppleReceiptVerificator = appleReceiptVerificator;
        }

        public async Task Intercept(string body)
        {

            await AppleReceiptVerificator.VerifyAppleReceiptAsync(null);

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
