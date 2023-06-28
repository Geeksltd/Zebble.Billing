namespace Zebble.Billing
{
    using Newtonsoft.Json;
    using Olive;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    class SubscriptionFileStore
    {
        static BillingContext Context => BillingContext.Current;
        static Subscription Subscription => Context?.Subscription;

        static FileInfo GetFile(IBillingUser user)
            => Device.IO.Cache.GetFile($"{user.UserId}-Billing-v1.json");

        public static void Load(IBillingUser user)
        {
            if (user is null) throw new ArgumentNullException(nameof(user));

            Context.IsLoaded = true;

            var file = GetFile(user);
            if (!file.Exists()) return;

            var fileContents = file.ReadAllText();
            if (fileContents.IsEmpty()) return;

            Context.Subscription = JsonConvert.DeserializeObject<Subscription>(fileContents);
        }

        public static async Task Save(IBillingUser user)
        {
            if (user is null) throw new ArgumentNullException(nameof(user));

            var fileContents = Subscription is null ? null : JsonConvert.SerializeObject(Subscription);
            await GetFile(user).WriteAllTextAsync(fileContents ?? "");
        }
    }
}
