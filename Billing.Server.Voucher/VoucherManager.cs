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
        readonly ISubscriptionComparer SubscriptionComparer;
        readonly VoucherConnector StoreConnector;

        public VoucherManager(IVoucherRepository voucherRepository, ISubscriptionRepository subscriptionRepository, ISubscriptionComparer subscriptionComparer, VoucherConnector storeConnector)
        {
            VoucherRepository = voucherRepository;
            SubscriptionRepository = subscriptionRepository;
            SubscriptionComparer = subscriptionComparer;
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

        public async Task<ApplyVoucherResult> Apply(string userId, string code)
        {
            var voucher = await VoucherRepository.GetByCode(code);

            if (voucher is null) return ApplyVoucherResult.From(VoucherApplyStatus.InvalidCode);

            if (voucher.UserId.Or(userId) != userId) return ApplyVoucherResult.From(VoucherApplyStatus.InvalidCode);

            if (voucher.IsExpired()) return ApplyVoucherResult.From(VoucherApplyStatus.Expired);

            voucher.UserId = userId;
            voucher.ActivationDate ??= LocalTime.UtcNow;

            await VoucherRepository.Update(voucher);

            await CreateSubscription(voucher);

            return ApplyVoucherResult.Succeeded();
        }

        async Task CreateSubscription(Voucher voucher)
        {
            var subscriptionInfo = await StoreConnector.GetSubscriptionInfo(voucher.ToArgs());
            if (subscriptionInfo.Status != SubscriptionQueryStatus.Succeeded) throw new Exception("Couldn't find voucher info.");

            var subscriptions = await SubscriptionRepository.GetAllWithTransactionId(subscriptionInfo.TransactionId);
            var subscription = subscriptions.Where(x => x.UserId == voucher.UserId).GetMostRecent(SubscriptionComparer);

            if (subscription is null)
            {
                subscription = await SubscriptionRepository.AddSubscription(new Subscription
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = voucher.ProductId,
                    UserId = subscriptionInfo.UserId,
                    Platform = "Voucher",
                    TransactionId = subscriptionInfo.TransactionId,
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
