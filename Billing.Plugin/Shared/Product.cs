namespace Zebble.Billing
{
#if CAFEBAZAAR
    using Plugin.InAppBilling.Abstractions;
#else
    using Plugin.InAppBilling;
#endif
    using Zebble.Device;

    public partial class Product
    {
        internal ItemType ItemType => Type.ToItemType();

        public decimal Price { get; set; }
        public string LocalPrice
        {
            get
            {
#if CAFEBAZAAR
                return $"{Price / 1000} هزار تومان";
#else
                return $"${Price}";
#endif
            }
        }

        protected string PaymentAuthority
        {
            get
            {
#if CAFEBAZAAR
                return "Cafe Bazaar";
#else
                return OS.Platform switch
                {
                    DevicePlatform.IOS => "iTunes",
                    DevicePlatform.Android => "Google Play",
                    _ => "Windows Store",
                };
#endif
            }
        }

        public override string ToString() => Title;
    }
}