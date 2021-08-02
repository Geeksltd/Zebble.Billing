namespace Zebble.Billing
{
    using Microsoft.Extensions.Logging;
    using Olive;
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
                return SubscriptionInfo.NotFound;
            }

            return CreateSubscription(args.UserId, result);
        }

        SubscriptionInfo CreateSubscription(string userId, Voucher voucher)
        {
            return new SubscriptionInfo
            {
                UserId = userId.Or(voucher.UserId),
                TransactionId = voucher.Id,
                SubscriptionDate = voucher.ActivationDate,
                ExpirationDate = voucher.ExpirationDate()
            };
        }
    }
}
