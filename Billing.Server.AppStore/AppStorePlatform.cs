namespace Zebble.Billing
{
    abstract class AppStorePlatform : IPlatformAware
    {
        public string Platform => "AppStore";
    }
}
