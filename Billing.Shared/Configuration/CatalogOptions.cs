namespace Zebble.Billing
{
    using System;
    using System.Collections.Generic;
    using Olive;

    public class CatalogOptions
    {
        public IEnumerable<Product> Products { get; set; }

        internal bool Validate()
        {
            if (Products == null) throw new ArgumentNullException(nameof(Products));
            if (Products.None()) throw new Exception($"{nameof(Products)} is empty.");

            Products.Do(x => x.Validate());

            return true;
        }
    }
}
