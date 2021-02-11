namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public partial interface IProductProvider<T>
    {
        Task<T[]> GetProducts();
        Task<T> GetById(string productId);
    }
}
