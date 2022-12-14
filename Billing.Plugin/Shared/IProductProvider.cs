namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public partial interface IProductProvider
    {
        Task UpdatePrice(string productId, decimal originalMicrosPrice, decimal discountedMicrosPrice, string currencyCode);
    }
}
