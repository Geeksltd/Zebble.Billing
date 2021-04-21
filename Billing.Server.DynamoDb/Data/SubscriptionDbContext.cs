namespace Zebble.Billing
{
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.DataModel;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq;
    using Olive;

    class SubscriptionDbContext : DynamoDBContext
    {
        public SubscriptionDbContext(IAmazonDynamoDB client) : base(client) { }

        public DbIndex<T> Index<T>(string name) => new(this, name);

        public DbTable<T> Table<T>() => new(this);

        public class DbIndex<T>
        {
            readonly IDynamoDBContext Db;
            readonly DynamoDBOperationConfig Config;

            public DbIndex(IDynamoDBContext db, string indexName)
            {
                Db = db;
                Config = new DynamoDBOperationConfig { IndexName = indexName };
            }

            public async Task<T[]> All(object hashKey)
            {
                var search = Db.QueryAsync<T>(hashKey, Config);
                return (await search.GetRemainingAsync())?.ToArray() ?? new T[0];
            }

            public Task<T> FirstOrDefault(string hashKey) => All(hashKey).FirstOrDefault();
        }

        public class DbTable<T>
        {
            readonly IDynamoDBContext Db;

            public DbTable(IDynamoDBContext db) => Db = db;

            public Task<List<T>> All(params ScanCondition[] conditions) => Db.ScanAsync<T>(conditions).GetRemainingAsync();

            public async Task<T> Load(string hash) => await Db.LoadAsync<T>(hash);

            public async Task<T> FirstOrDefault(params ScanCondition[] conditions)
            {
                var scan = Db.ScanAsync<T>(conditions);

                try
                {
                    return (await scan.GetNextSetAsync()).FirstOrDefault();
                }
                catch
                {
                    return default;
                }
            }
        }
    }
}
