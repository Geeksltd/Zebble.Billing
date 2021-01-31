namespace Zebble.Billing
{
    using Olive;
#if CAFEBAZAAR
    using Plugin.InAppBilling.Abstractions;
#else
    using Plugin.InAppBilling;
#endif
    using System;
    using System.IO;
    using System.Linq;
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

        internal void Reload(string data)
        {
            var parts = data.Split('|').Trim().ToArray();
            if (parts.Length != 1) return;

            Price = parts[0].To<decimal>();
        }

        internal void UpdatePrice(decimal price)
        {
            Price = price;
        }

        protected virtual string PaymentAuthority
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