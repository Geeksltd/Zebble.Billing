namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;

    class VoucherConnector : IStoreConnector
    {
        readonly IVoucherRepository Repository;

        public VoucherConnector(IVoucherRepository repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task<PurchaseVerificationStatus> VerifyPurchase(VerifyPurchaseArgs args) => Task.FromResult(PurchaseVerificationStatus.Verified);

        public async Task<SubscriptionInfo> GetSubscriptionInfo(SubscriptionInfoArgs args)
        {
            var result = await Repository.GetByCode(args.PurchaseToken);

            if (result is null) return null;
            return CreateSubscription(result);
        }

        SubscriptionInfo CreateSubscription(Voucher voucher)
        {
            return new SubscriptionInfo
            {
                UserId = voucher.UserId,
                TransactionId = voucher.Id,
                SubscriptionDate = voucher.ActivationDate,
                ExpirationDate = voucher.ExpirationDate(),
                CancellationDate = null,
                AutoRenews = false
            };
        }
    }
}
