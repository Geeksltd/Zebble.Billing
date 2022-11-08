using System.Collections.Generic;
using System.Linq;

namespace Zebble.Billing
{
    public static class SubscriptionExtensions
    {
        public static Subscription GetMostRecent(this IEnumerable<Subscription> @this, ISubscriptionComparer comparer)
            => @this.OrderBy(x => x, comparer).LastOrDefault();
    }
}
