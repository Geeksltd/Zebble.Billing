namespace Zebble.Billing
{
#if CAFEBAZAAR
    using Plugin.InAppBilling.Abstractions;
#else
    using Plugin.InAppBilling;
#endif
    using System.Threading.Tasks;

    static class ProductExtensions
    {
        public static Task<Product> GetProduct(this InAppBillingPurchase @this)
        {
            return @this.ProductId.GetProduct();
        }

        public static Task<Product> GetProduct(this string @this)
        {
            return BillingContext.ProductProvider.GetById(@this);
        }
    }
}