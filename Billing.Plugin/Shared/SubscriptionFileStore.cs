namespace Zebble.Billing
{
    using Newtonsoft.Json;
    using Olive;
    using System.IO;
    using System.Threading.Tasks;

    class SubscriptionFileStore
    {
        static BillingContext Context => BillingContext.Current;
        static IBillingUser User => Context?.User;
        static Subscription Subscription => Context?.Subscription;

        static FileInfo CacheFile => Device.IO.File($"{User.UserId}-Billing-v1.json");

        public static async Task Load()
        {
            while (User == null) await Task.Delay(500);
            if (!await CacheFile.ExistsAsync()) return;

            var fileContents = await CacheFile.ReadAllTextAsync();
            if (fileContents.IsEmpty()) return;

            Context.Subscription = JsonConvert.DeserializeObject<Subscription>(fileContents);
        }

        public static async Task Save()
        {
            if (User is null) throw new System.Exception("User is unknown.");

            var fileContents = Subscription is null ? null : JsonConvert.SerializeObject(Subscription);
            await CacheFile.WriteAllTextAsync(fileContents ?? "");
        }
    }
}
