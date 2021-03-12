namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public partial interface IProductProvider
    {
        Task<Product[]> GetProducts(string platform);
        Task<Product> GetById(string platform, string productId);
    }
}
