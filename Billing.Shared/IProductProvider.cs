namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public interface IProductProvider
    {
        Task<Product[]> GetProducts();
        Task<Product> GetById(string productId);
    }
}
