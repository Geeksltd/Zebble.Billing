namespace Zebble.Billing
{
    using Olive;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public static partial class BillingContext
    {
        static SubscriptionStatus? GetStatus()
        {
            if (User == null) return null;

            if (User.EverSubscribed || User.SubscriptionTokens.OrEmpty().Any() || User.VoucherCode.HasValue())
            {
                if (User.SubscriptionExpiry > LocalTime.UtcNow) return SubscriptionStatus.Subscribed;
                else return SubscriptionStatus.Expired;
            }

            return SubscriptionStatus.None;
        }

        public static bool IsExpired() => GetStatus() == SubscriptionStatus.Expired;

        public static bool IsSubscribed(bool free = true)
        {
            if (GetStatus() == SubscriptionStatus.Subscribed) return true;
            if (!free) return false;
            return new[] { SubscriptionStatus.FreeStarted, SubscriptionStatus.FreeExpiring }.Contains(GetStatus());
        }

        public static bool HasEssential() => IsSubscribed();

        public static bool HasPro()
        {
            return IsSubscribed() && (User.SubscriptionType?.HasFlag(SubscriptionType.Pro) ?? true);
        }

        public static async Task<string> Subscribe(Product product)
        {
            return await new SubscribeCommand(product).Execute()
                 ?? "Failed to connect to the store. Are you connected to the network? If so, try 'Pay with Card'.";
        }

        public static async Task<bool> RestoreSubscriptions(bool userRequest = false)
        {
            var errorMessage = "";
            try { await new RestoreSubscriptionCommand().Execute(); }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                Log.For(typeof(BillingContext)).Error(ex);
            }

            var successful = false;
            try { successful = await TryActivate(); }
            catch (Exception ex)
            {
                if (errorMessage.IsEmpty()) errorMessage = ex.Message;
                Log.For(typeof(BillingContext)).Error(ex);
            }

            if (!successful && userRequest)
            {
                if (errorMessage.IsEmpty()) errorMessage = "Unable to find an active subscription.";
                await Alert.Show(errorMessage);
            }

            return successful;
        }

        static async Task<bool> TryActivate()
        {
            await Refresh();
            return User?.SubscriptionExpiry > LocalTime.UtcNow;
        }

        public static async Task UpdatesPrices()
        {
            if (ProductsCache.ArePricesUpToDate()) return;

            await UIContext.AwaitConnection(10);
            await Task.Delay(3.Seconds());

            try { await new SubscriptionPriceProvider().Execute(); }
            catch (Exception ex) { Log.For(typeof(BillingContext)).Error(ex); }
        }
    }
}
