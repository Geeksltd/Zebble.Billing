namespace Zebble.Billing
{
    using System.Threading.Tasks;

    partial class BillingContext
    {
        public Task LaunchManageSubscriptions()
        {
            return Zebble.Device.OS.OpenBrowser(
                "https://apps.apple.com/account/subscriptions",
                OnError.Toast);
        }
    }
}
