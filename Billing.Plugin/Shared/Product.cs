namespace Zebble.Billing
{
    using Plugin.InAppBilling;

    public partial class Product
    {
        public ProductType Type { get; set; }

        public decimal Price { get; set; }

        public override string ToString() => Id;
    }
}