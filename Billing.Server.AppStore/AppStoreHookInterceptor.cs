namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Apple.Receipt.Verificator.Services;
    using Apple.Receipt.Verificator.Models.IAPVerification;

    class AppStoreHookInterceptor
    {
        readonly AppStoreOptions Options;
        readonly ISubscriptionRepository Repository;
        readonly IAppleReceiptVerificatorService AppleReceiptVerificator;

        public AppStoreHookInterceptor(IOptionsSnapshot<AppStoreOptions> options, ISubscriptionRepository repository, IAppleReceiptVerificatorService appleReceiptVerificator)
        {
            Options = options.Value;
            Repository = repository;
            AppleReceiptVerificator = appleReceiptVerificator;
        }

        public async Task Intercept(string body)
        {
            var notification = body.ToNotification();

            ValidateNotification(notification);

            await AppleReceiptVerificator.VerifyAppleReceiptAsync(notification.Receipt.);

            await Repository.AddTransaction(new Transaction
            {
                TransactionId = Guid.NewGuid().ToString(),
                //SubscriptionId = subscription.SubscriptionId,
                Platform = "AppStore",
                //Date = notification.EventTime,
                //Details = notification.OriginalData
            });
        }

        void ValidateNotification(IAPVerificationResponse notification)
        {
            if (notification.Environment != Options.Environment) throw new Exception("Environment doesn't match.");
        }
    }
}
