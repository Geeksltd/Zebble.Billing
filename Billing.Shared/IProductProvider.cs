namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public partial interface IProductProvider
    {
        Task<Product[]> GetProducts();
        Task<Product> GetById(string productId);
    }
}
