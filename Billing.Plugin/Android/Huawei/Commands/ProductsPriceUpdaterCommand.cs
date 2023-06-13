namespace Zebble.Billing
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Huawei.Hmf.Extensions;
    using Huawei.Hms.Iap.Entity;
    using Olive;

    class ProductsPriceUpdaterCommand : StoreCommandBase<bool>
    {
        protected override async Task<bool> DoExecute(IBillingUser user)
        {
            try
            {
                var productProvider = BillingContext.Current.ProductProvider;
                var products = await productProvider.GetProducts();

                return await ProcessProducts(products);
            }
            catch (Exception ex)
            {
                Log.For(this).Error(ex);
                throw;
            }
        }

        async Task<bool> ProcessProducts(Product[] products)
        {
            var productProvider = BillingContext.Current.ProductProvider;
            var groups = products.GroupBy(x => x.GetPriceType())
                .Select(x => new { PriceType = x.Key, ProductIds = x.Select(p => p.Id).ToArray() });

            foreach (var group in groups)
            {
                var request = new ProductInfoReq
                {
                    PriceType = group.PriceType,
                    ProductIds = group.ProductIds
                };

                var result = await Billing.ObtainProductInfo(request).AsAsync<ProductInfoResult>();

                var items = result.ProductInfoList;
                if (items == null)
                    throw new Exception($"No product info was retrieved for {group.ProductIds.ToString(", ")} ({group.PriceType})");

                foreach (var item in items)
                {
                    var discountedMicrosPrice = item.SubSpecialPriceMicros;
                    if (discountedMicrosPrice == default) discountedMicrosPrice = item.MicrosPrice;

                    await productProvider.UpdatePrice(item.ProductId, item.MicrosPrice, discountedMicrosPrice, item.Currency);
                }
            }

            return true;
        }
    }
}