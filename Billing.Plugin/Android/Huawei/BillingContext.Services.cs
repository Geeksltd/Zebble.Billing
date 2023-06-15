namespace Zebble.Billing
{
    using Android.Content;
    using Huawei.Agconnect.Config;

    public partial class BillingContext
    {
        static bool IsConfigured;

        public static void ConfigureServices(Context context)
        {
            if (IsConfigured) return;

            AGConnectServicesConfig.FromContext(context)
                .OverlayWith(new HmsLazyInputStream(context));

            IsConfigured = true;
        }
    }
}
