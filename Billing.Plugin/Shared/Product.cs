namespace Zebble.Billing
{
	public partial class Product
	{
		public decimal Price { get; set; }
		public string CurrencySymbol { get; set; }
		
		public string LocalPrice => $"{CurrencySymbol}{Price}";
	}
}