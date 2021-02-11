namespace Zebble.Billing
{
    using Android.App;
    using Android.OS;

    partial class BillingContext
    {
        public static void InitApp(Application app)
        {
            Xamarin.Essentials.Platform.Init(app);
        }

        public static void InitActivity(Activity activity, Bundle bundle)
        {
            Xamarin.Essentials.Platform.Init(activity, bundle);
        }

        public static string PaymentAuthority => "GooglePlay";
    }
}
