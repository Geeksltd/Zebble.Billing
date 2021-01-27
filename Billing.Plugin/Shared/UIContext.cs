namespace Zebble.Billing
{
    using Olive;
    using System.Threading.Tasks;
    using Zebble.Device;

    class UIContext
    {
        public static async Task<bool> IsOffline() => !await IsOnline();

        public static Task<bool> IsOnline() => Network.IsAvailable();

        internal static async Task AwaitConnection(int checkIntervals = 2)
        {
            while (await IsOffline())
                await Task.Delay(checkIntervals.Seconds());
        }
    }
}
