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

    public abstract class Product
    {
        public abstract ProductType Type { get; }
        internal ItemType ItemType => Type.ToItemType();
        public abstract string Id { get; }
        public abstract string Title { get; }
        public abstract int Months { get; }
        public abstract string Promo { get; }
        public abstract int FreeDays { get; }
        public abstract SubscriptionType SubscriptionType { get; }
        public abstract bool IsLifetime { get; }

        public decimal Price { get; protected set; }
        public virtual string LocalPrice => $"${Price}";

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

        protected virtual string PaymentAuthority => OS.Platform switch
        {
            DevicePlatform.IOS => "iTunes",
            DevicePlatform.Android => "Google Play",
            _ => "Windows Store",
        };

        public override string ToString() => Title;
    }
}