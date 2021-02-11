namespace Zebble.Billing
{
    using System.Collections.Generic;

    public class CatalogOptions<T> where T : Product
    {
        public IEnumerable<T> Products { get; set; }
    }
}
