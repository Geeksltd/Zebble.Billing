namespace Zebble.Billing
{
    using Android.Content;
    using Huawei.Agconnect.Config;

    public partial class BillingContext
    {
        public static void ConfigureServices(Context context)
        {
            AGConnectServicesConfig.FromContext(context)
                .OverlayWith(new HmsLazyInputStream(context));
        }
    }
}
