namespace Zebble.Billing
{
    using System.Threading.Tasks;

    partial class BillingContext
    {
        public Task LaunchManageSubscriptions()
            => Task.CompletedTask;
    }
}
