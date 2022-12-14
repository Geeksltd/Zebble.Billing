namespace Zebble.Billing
{
    public partial class Product
    {
        public decimal OriginalPrice { get; set; }
        public string LocalOriginalPrice => $"{CurrencySymbol}{OriginalPrice}";

        public decimal DiscountedPrice { get; set; }
        public string LocalDiscountedPrice => $"{CurrencySymbol}{DiscountedPrice}";

        public string CurrencySymbol { get; set; }

        public bool HasDiscount => DiscountedPrice < OriginalPrice;
    }
}