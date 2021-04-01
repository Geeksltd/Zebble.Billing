namespace Zebble.Billing
{
    using System;
    using System.Threading.Tasks;
    using Olive;

    class VoucherCodeApplier
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
            voucher.ActivationDate ??= LocalTime.UtcNow;

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
            if (voucher is null) throw new Exception("No voucher found.");

            if (voucher.ActivationDate.HasValue && voucher.UserId != userId) throw new Exception("This voucher is already applied.");
        }

        async Task CreateSubscription(Voucher voucher)
        {
            var subscription = await SubscriptionRepository.GetByPurchaseToken(voucher.Code);

            if (subscription is null)
            {
                var subscriptionInfo = await StoreConnector.GetSubscriptionInfo(voucher.ToArgs());

                if (subscriptionInfo is null) throw new Exception("Couldn't find voucher info.");

                subscription = await SubscriptionRepository.AddSubscription(new Subscription
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = voucher.ProductId,
                    UserId = subscriptionInfo.UserId,
                    Platform = "Voucher",
                    TransactionId = subscriptionInfo.TransactionId,
                    ReceiptData = voucher.Code,
                    TransactionDate = voucher.ActivationDate,
                    PurchaseToken = voucher.Code,
                    SubscriptionDate = subscriptionInfo.SubscriptionDate,
                    ExpirationDate = subscriptionInfo.ExpirationDate,
                    CancellationDate = subscriptionInfo.CancellationDate,
                    LastUpdate = LocalTime.UtcNow,
                    AutoRenews = subscriptionInfo.AutoRenews
                });
            }

            await SubscriptionRepository.AddTransaction(new Transaction
            {
                Id = Guid.NewGuid().ToString(),
                SubscriptionId = subscription.Id,
                Platform = "Voucher",
                Date = LocalTime.UtcNow,
                Details = voucher.ToJson()
            });
        }
    }
}
