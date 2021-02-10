namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Olive;

    public class VoucherCodeApplier
    {
        readonly IVoucherRepository VoucherRepository;
        readonly ISubscriptionRepository SubscriptionRepository;
        readonly VoucherConnector StoreConnector;

        public VoucherCodeApplier(IVoucherRepository voucherRepository, ISubscriptionRepository subscriptionRepository, VoucherConnector storeConnector)
        {
            VoucherRepository = voucherRepository;
            SubscriptionRepository = subscriptionRepository;
            StoreConnector = storeConnector;
        }

        public async Task Apply(string userId, string code)
        {
            var voucher = await FindVoucher(userId, code);

            voucher.UserId = userId;
            voucher.ActivationDate ??= LocalTime.Now;

            await VoucherRepository.Update(voucher);

            await CreateSubscription(voucher);
        }

        async Task<Voucher> FindVoucher(string userId, string code)
        {
            var voucher = await VoucherRepository.GetByCode(code);

            ValidateVoucher(voucher, userId);

            return voucher;
        }

        void ValidateVoucher(Voucher voucher, string userId)
        {
            if (voucher == null) throw new Exception("No voucher found.");

            if (voucher.ActivationDate.HasValue && voucher.UserId != userId) throw new Exception("This voucher is already applied.");
        }

        async Task CreateSubscription(Voucher voucher)
        {
            var subscription = await SubscriptionRepository.GetByPurchaseToken(voucher.Code);

            if (subscription == null)
            {
                subscription = await StoreConnector.GetUpToDateInfo(voucher.ProductId, voucher.Code);

                if (subscription == null)
                    throw new Exception("Couldn't convert voucher to a subscription.");

                subscription = await SubscriptionRepository.AddSubscription(subscription);
            }

            await SubscriptionRepository.AddTransaction(new Transaction
            {
                TransactionId = Guid.NewGuid().ToString(),
                SubscriptionId = subscription.SubscriptionId,
                Platform = "Voucher",
                Date = LocalTime.Now,
                Details = voucher.ToJson()
            });
        }
    }
}
