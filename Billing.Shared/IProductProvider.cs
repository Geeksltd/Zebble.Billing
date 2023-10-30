namespace Zebble.Billing
{
    public partial interface IProductProvider
    {
        Product[] GetProducts();
        Product GetById(string productId);
    }
}
