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

        public override string ToString() => Title;
    }
}