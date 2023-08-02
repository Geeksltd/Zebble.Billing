namespace Zebble.Billing
{
    using System.Threading.Tasks;

    partial class BillingContext
    {
        public Task LaunchManageSubscriptions()
        {
            return Zebble.Device.OS.OpenBrowser(
                "https://play.google.com/store/account/subscriptions",
                OnError.Toast);
        }
    }
}
