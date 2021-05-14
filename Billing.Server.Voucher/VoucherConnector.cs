namespace Zebble.Billing
{
    using Olive;
    using System;
    using System.Threading.Tasks;

    class VoucherConnector : IStoreConnector
    {
        readonly IVoucherRepository Repository;

        public VoucherConnector(IVoucherRepository repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<SubscriptionInfo> GetSubscriptionInfo(SubscriptionInfoArgs args)
        {
            var result = await Repository.GetByCode(args.PurchaseToken);

            if (result is null) return SubscriptionInfo.NotFound;
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
