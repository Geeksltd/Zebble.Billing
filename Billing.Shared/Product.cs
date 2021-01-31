namespace Zebble.Billing
{
    public partial class Product
    {
        public string Id { get; set; }
        public SubscriptionPlatform? Platform { get; set; }
        public ProductType Type { get; set; }
        public string Title { get; set; }
        public int Months { get; set; }
        public string Promo { get; set; }
        public int FreeDays { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
        public bool IsLifetime { get; set; }
        public decimal Price { get; set; }
    }
}
