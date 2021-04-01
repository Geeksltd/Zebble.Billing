namespace Zebble.Billing
{
    using System.Threading.Tasks;
    using Olive;

    class VoucherConnector : IStoreConnector
    {
        readonly IVoucherRepository Repository;

        public VoucherConnector(IVoucherRepository repository)
        {
            Repository = repository;
        }

        public Task<PurchaseVerificationStatus> VerifyPurchase(VerifyPurchaseArgs args)
        {
            return Task.FromResult(PurchaseVerificationStatus.Verified);
        }

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
                SubscriptionDate = voucher.ActivationDate ?? LocalTime.UtcNow,
                ExpirationDate = voucher.ExpirationDate(),
                CancellationDate = null,
                AutoRenews = false
            };
        }
    }
}
