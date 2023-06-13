namespace Zebble.Billing
{
    using System.Threading.Tasks;

    public class HuaweiDeveloperService
    {
        public async Task<HuaweiValidatePurchaseResult> ValidatePurchase(
            HuaweiValidatePurchaseRequest request)
        {
            return new HuaweiValidatePurchaseResult();
        }
    }
}
