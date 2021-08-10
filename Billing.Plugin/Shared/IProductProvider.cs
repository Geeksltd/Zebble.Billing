namespace Zebble.Billing
{
	using System.Threading.Tasks;

	public partial interface IProductProvider
	{
		Task UpdatePrice(string productId, decimal microsPrice, string currencyCode);
	}
}
