namespace Zebble.Billing
{
	partial class Product
	{
		public string LocalPrice => $"{CurrencySymbol}{Price}";
	}
}