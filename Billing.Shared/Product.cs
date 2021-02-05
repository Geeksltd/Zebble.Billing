namespace Zebble.Billing
{
    public partial class Product
    {
        public string Id { get; set; }
        public string Platform { get; set; }
        public ProductType Type { get; set; }
        public string Title { get; set; }
        public int Months { get; set; }
        public string Promo { get; set; }
        public int FreeDays { get; set; }
    }
}
