namespace Zebble.Billing
{
    using Android.App;
    using Android.Content;
    using Android.OS;

    partial class BillingContext<T>
    {
        public static void InitApp(Application app)
        {
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(app);
        }

        public static void InitActivity(Activity activity, Bundle bundle)
        {
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(activity, bundle);
        }

        public static void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            Plugin.InAppBilling.InAppBillingImplementation.HandleActivityResult(requestCode, resultCode, data);
        }

        public static string PaymentAuthority => "CafeBazaar";
    }
}
