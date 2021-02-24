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

        public Task<bool> VerifyPurchase(string productId, string receiptData)
        {
            return Task.FromResult(true);
        }

        public async Task<SubscriptionInfo> GetUpToDateInfo(string productId, string purchaseToken)
        {
            var result = await Repository.GetByCode(purchaseToken);

            if (result == null)
                return null;

            return CreateSubscription(result);
        }

        SubscriptionInfo CreateSubscription(Voucher voucher)
        {
            return new SubscriptionInfo
            {
                UserId = voucher.UserId,
                TransactionId = voucher.Id,
                SubscriptionDate = voucher.ActivationDate ?? LocalTime.Now,
                ExpirationDate = voucher.ExpirationDate(),
                CancellationDate = null,
                AutoRenews = false
            };
        }
    }
}
