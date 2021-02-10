namespace Zebble.Billing
{
    using Plugin.InAppBilling;

    public partial class Product
    {
        internal ItemType ItemType => Type.ToItemType();

        public decimal Price { get; set; }

        public override string ToString() => Title;
    }
}