namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public partial interface IProductProvider<T>
    {
        Task UpdatePrice(string productId, decimal price);
    }
}
