namespace Zebble.Billing
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;

    class VoucherConnector : IStoreConnector
    {
        readonly ILogger<VoucherConnector> Logger;
        readonly IVoucherRepository Repository;

        public VoucherConnector(ILogger<VoucherConnector> logger, IVoucherRepository repository)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<SubscriptionInfo> GetSubscriptionInfo(SubscriptionInfoArgs args)
        {
            var result = await Repository.GetByCode(args.PurchaseToken);

            if (result is null)
            {
                Logger.LogWarning($"No voucher with code '{args.PurchaseToken}' found.");
                return null;
            }

            return CreateSubscription(result);
        }

        SubscriptionInfo CreateSubscription(Voucher voucher)
        {
            return new SubscriptionInfo
            {
                ProductId = voucher.ProductId,
                TransactionId = voucher.Id,
                SubscriptionDate = voucher.ActivationDate,
                ExpirationDate = voucher.ExpirationDate()
            };
        }
    }
}
