﻿namespace Zebble.Billing
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
        readonly ISubscriptionChangeHandler SubscriptionChangeHandler;

        public VoucherManager(
            IVoucherRepository voucherRepository,
            ISubscriptionRepository subscriptionRepository,
            ISubscriptionComparer subscriptionComparer,
            VoucherConnector storeConnector,
            ISubscriptionChangeHandler subscriptionChangeHandler
        )
        {
            VoucherRepository = voucherRepository ?? throw new ArgumentNullException(nameof(voucherRepository));
            SubscriptionRepository = subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
            SubscriptionComparer = subscriptionComparer ?? throw new ArgumentNullException(nameof(subscriptionComparer));
            StoreConnector = storeConnector ?? throw new ArgumentNullException(nameof(storeConnector));
            SubscriptionChangeHandler = subscriptionChangeHandler ?? throw new ArgumentNullException(nameof(subscriptionChangeHandler));
        }

        public async Task<string> Generate(TimeSpan duration, string productId, string comments, string discountCode)
        {
            const string UNAMBIGUOUS_LETTERS = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var code = Enumerable.Range(0, 12).Select(x => UNAMBIGUOUS_LETTERS.PickRandom()).ToString("");

            if (await VoucherRepository.GetByCode(code) is not null) return await Generate(duration, productId, comments, discountCode);

            await VoucherRepository.Add(new Voucher
            {
                Id = Guid.NewGuid().ToString(),
                Code = code,
                Duration = duration,
                ProductId = productId,
                Comments = comments,
                DiscountCode = discountCode,
            });

            return code;
        }

        public async Task<ApplyVoucherResult> Apply(string userId, string code, DateTime? activationDate = null)
        {
            var voucher = await VoucherRepository.GetByCode(code);

            if (voucher is null) return ApplyVoucherResult.From(VoucherApplyStatus.InvalidCode);

            if (voucher.UserId.Or(userId) != userId) return ApplyVoucherResult.From(VoucherApplyStatus.InvalidCode);

            if (voucher.IsExpired()) return ApplyVoucherResult.From(VoucherApplyStatus.Expired);

            voucher.UserId = userId;
            voucher.ActivationDate ??= activationDate ?? LocalTime.UtcNow;

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
                    SubscriptionDate = subscriptionInfo.SubscriptionDate.HasValue ? subscriptionInfo.SubscriptionDate : LocalTime.UtcNow,
                    ExpirationDate = subscriptionInfo.ExpirationDate.HasValue ? subscriptionInfo.ExpirationDate : LocalTime.UtcNow.Add(voucher.Duration),
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

            await SubscriptionChangeHandler.Handle(subscription);
        }
    }
}
