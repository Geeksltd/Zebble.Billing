namespace Zebble.Billing
{
    using Amazon.DynamoDBv2.DataModel;
    using Olive;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using Expression = System.Linq.Expressions.Expression;

    public static class DynamoDBContextExtensions
    {
        public static Task<List<T>> Where<T>(this DynamoDBContext db, params ScanCondition[] conditions)
        {
            return db.ScanAsync<T>(conditions, null).GetRemainingAsync();
        }

        public static async Task<T> FirstOrDefault<T>(this DynamoDBContext db, params ScanCondition[] conditions)
        {
            return (await db.Where<T>(conditions)).FirstOrDefault();
        }

        public static async Task UpdateAsync<T>(this DynamoDBContext db, Expression<Func<T, object>> hashKeySelector, T updatedRecord)
        {
            var hasKey = Expression.Invoke(hashKeySelector, Expression.Constant(updatedRecord));
            var hasKeyName = ((MemberExpression)hashKeySelector.Body).Member.Name;

            var dbRecord = await db.LoadAsync<T>(hasKey);

            CopyProperties(dbRecord, updatedRecord, hasKeyName);

            await db.SaveAsync(dbRecord);
        }

        static void CopyProperties<T>(T dbSubscription, T subscription, params string[] excludedProps)
        {
            typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                     .Where(x => x.CanRead && x.CanWrite)
                     .Where(x => !excludedProps.Contains(x.Name))
                     .Do(p => p.SetValue(dbSubscription, p.GetValue(subscription)));
        }
    }
}
