namespace Zebble.Billing
{
	using Amazon.DynamoDBv2;
	using Amazon.DynamoDBv2.DataModel;
	using System.Threading.Tasks;
	using System.Linq;
	using Olive;
	using System;
	using System.Linq.Expressions;

	public abstract class BillingDynamoDbContext : DynamoDBContext
	{
		protected BillingDynamoDbContext(IAmazonDynamoDB client) : base(client) { }

		protected DbIndex<T> Index<T>(string name) => new(this, name);

		protected DbTable<T> Table<T>() => new(this);

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

			public Task AddAsync(T newRecord) => Db.SaveAsync(newRecord);

			public async Task UpdateAsync(Expression<Func<T, string>> hashSelector, T updatedRecord)
			{
				var hash = (string)Expression.Invoke(hashSelector, Expression.Constant(updatedRecord)).CompileAndInvoke();
				var hashName = ((MemberExpression)hashSelector.Body).Member.Name;

				var dbRecord = await Load(hash);

				if (dbRecord is null) dbRecord = updatedRecord;
				else ReflectionExtensions.CopyPropertiesFrom(dbRecord, updatedRecord, hashName);

				await Db.SaveAsync(dbRecord);
			}

			async Task<T> Load(string hash) => await Db.LoadAsync<T>(hash);
		}
	}
}
