namespace Zebble.Billing
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Olive;

    class VoucherManager : IVoucherManager
    {
        readonly IVoucherRepository VoucherRepository;
        readonly ISubscriptionRepository SubscriptionRepository;
        readonly VoucherConnector StoreConnector;

        public VoucherManager(IVoucherRepository voucherRepository, ISubscriptionRepository subscriptionRepository, VoucherConnector storeConnector)
        {
            VoucherRepository = voucherRepository;
            SubscriptionRepository = subscriptionRepository;
            StoreConnector = storeConnector;
        }

        public async Task<string> Generate(TimeSpan duration, string productId, string comments)
        {
            const string UNAMBIGUOUS_LETTERS = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var code = Enumerable.Range(0, 12).Select(x => UNAMBIGUOUS_LETTERS.PickRandom()).ToString("");

            if (await VoucherRepository.GetByCode(code) is not null) return await Generate(duration, productId, comments);

            await VoucherRepository.Add(new Voucher
            {
                Id = Guid.NewGuid().ToString(),
                Code = code,
                Duration = duration,
                ProductId = productId,
                Comments = comments
            });

            return code;
        }

        public async Task Apply(string userId, string code)
        {
            var voucher = await FindVoucher(code);

            ValidateVoucher(voucher, userId);

            voucher.UserId = userId;
            voucher.ActivationDate ??= LocalTime.UtcNow;

            await VoucherRepository.Update(voucher);

            await CreateSubscription(voucher);
        }

        async Task<Voucher> FindVoucher(string code)
        {
            return await VoucherRepository.GetByCode(code) ?? throw new Exception($"No voucher found with code '{code}'.");
        }

        void ValidateVoucher(Voucher voucher, string userId)
        {
            if (voucher.UserId == userId) return;

            if (!voucher.ActivationDate.HasValue) return;

            throw new Exception($"Voucher with code '{voucher.Code}' is already applied for another user.");
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
