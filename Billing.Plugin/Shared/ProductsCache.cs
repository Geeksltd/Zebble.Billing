namespace Zebble.Billing
{
    using Olive;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Zebble.Device;

    static class ProductsCache
    {
        internal static readonly IList<Product> RegisteredProducts = new List<Product>();

        static FileInfo File => IO.File("Products.json");

        static ProductsCache()
        {
            if (File.Exists()) ReloadPrices();
        }

        static void ReloadPrices()
        {
            var lines = File.ReadAllText().Trim().ToLines().Trim().ToArray();
            if (lines.Length != RegisteredProducts.Count) return;

            RegisteredProducts.Do((p, i) => p.Reload(lines[i]));
        }

        public static bool ArePricesUpToDate()
        {
            if (!File.Exists()) return false;
            return File.LastWriteTimeUtc > LocalTime.UtcNow.AddDays(-10);
        }

        public static void SavePrices()
        {
            File.WriteAllText(RegisteredProducts.Select(v => $"{v.Price}").ToLinesString());
        }
    }
}