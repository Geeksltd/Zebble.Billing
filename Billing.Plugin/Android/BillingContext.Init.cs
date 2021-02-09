namespace Zebble.Billing
{
    using Android.App;
#if CAFEBAZAAR
    using Android.Content;
#endif
    using Android.OS;

    public static partial class BillingContext
    {
        public static void InitApp(Application app)
        {
#if CAFEBAZAAR
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(app);
#else
            Xamarin.Essentials.Platform.Init(app);
#endif
        }

        public static void InitActivity(Activity activity, Bundle bundle)
        {
#if CAFEBAZAAR
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(activity, bundle);
#else
            Xamarin.Essentials.Platform.Init(activity, bundle);
#endif
        }

#if CAFEBAZAAR
        public static void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            Plugin.InAppBilling.InAppBillingImplementation.HandleActivityResult(requestCode, resultCode, data);
        }
#endif
    }
}
