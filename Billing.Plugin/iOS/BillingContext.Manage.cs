namespace Zebble.Billing
{
    using System.Threading.Tasks;

    partial class BillingContext
    {
        public Task LaunchManageSubscriptions()
        {
            return Zebble.Device.OS.OpenBrowser(
                "https://buy.itunes.apple.com/WebObjects/MZFinance.woa/wa/manageSubscriptions",
                OnError.Toast);
        }
    }
}
