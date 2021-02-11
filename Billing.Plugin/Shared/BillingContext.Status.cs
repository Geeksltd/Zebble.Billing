namespace Zebble.Billing
{
    using System.Threading.Tasks;

    partial class BillingContext
    {
        public bool IsSubscribed => IsStarted && !IsExpired && !IsCanceled;

        public Task<Product> CurrentProduct => GetProduct(CurrentProductId);

        public string CurrentProductId => Subscription?.ProductId;

        public bool IsStarted => Subscription.IsStarted();

        public bool IsExpired => Subscription.IsExpired();

        public bool IsCanceled => Subscription.IsCanceled();
    }
}
