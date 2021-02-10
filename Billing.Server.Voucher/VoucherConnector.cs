namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Olive;

    public class VoucherConnector : IStoreConnector
    {
        readonly IVoucherRepository Repository;

        public VoucherConnector(IVoucherRepository repository)
        {
            Repository = repository;
        }

        public async Task<Subscription> GetUpToDateInfo(string productId, string purchaseToken)
        {
            var result = await Repository.GetByCode(purchaseToken);

            if (result == null)
                return null;

            return CreateSubscription(productId, purchaseToken, result);
        }

        Subscription CreateSubscription(string productId, string purchaseToken, Voucher voucher)
        {
            return new Subscription
            {
                SubscriptionId = Guid.NewGuid().ToString(),
                ProductId = productId,
                UserId = voucher.UserId,
                Platform = "Voucher",
                PurchaseToken = purchaseToken,
                OriginalTransactionId = voucher.Id,
                SubscriptionDate = voucher.ActivationDate ?? LocalTime.Now,
                ExpirationDate = voucher.ExpirationDate(),
                CancellationDate = null,
                LastUpdate = LocalTime.Now,
                AutoRenews = false
            };
        }
    }
}
