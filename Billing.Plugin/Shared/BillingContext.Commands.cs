namespace Zebble.Billing
{
    using Olive;
    using System;
    using System.Threading.Tasks;

    public static partial class BillingContext
    {
        public static async Task<string> PurchaseSubscription(Product product)
        {
            return await new PurchaseSubscriptionCommand(product).Execute()
                 ?? "Failed to connect to the store. Are you connected to the network? If so, try 'Pay with Card'.";
        }

        public static async Task<bool> RestoreSubscription(bool userRequest = false)
        {
            var errorMessage = "";
            try { await new RestoreSubscriptionCommand().Execute(); }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                Log.For(typeof(BillingContext)).Error(ex);
            }

            var successful = false;
            try
            {
                await Refresh();
                successful = IsSubscribed();
            }
            catch (Exception ex)
            {
                if (errorMessage.IsEmpty()) errorMessage = ex.Message;
                Log.For(typeof(BillingContext)).Error(ex);
            }

            if (!successful && userRequest)
            {
                if (errorMessage.IsEmpty()) errorMessage = "Unable to find an active subscription.";
                await Alert.Show(errorMessage);
            }

            return successful;
        }

        public static async Task UpdateProductPrices()
        {
            await UIContext.AwaitConnection(10);
            await Task.Delay(3.Seconds());

            try { await new ProductsPriceUpdaterCommand().Execute(); }
            catch (Exception ex) { Log.For(typeof(BillingContext)).Error(ex); }
        }
    }
}
