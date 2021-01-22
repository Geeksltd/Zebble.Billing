namespace Zebble.Billing
{
    using Android.App;
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
    }
}
