namespace Zebble.Billing
{
    public partial class Product
    {
        public string Id { get; set; }
        public string Platform { get; set; }
        public ProductType Type { get; set; }

        public override string ToString() => Id;
    }
}
