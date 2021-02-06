namespace Zebble.Billing
{
    using System;
    using Olive;

    public class Product
    {
        public string Id { get; set; }
        public string Platform { get; set; }
        public ProductType Type { get; set; }
        public string Title { get; set; }
        public int Months { get; set; }
        public string Promo { get; set; }
        public int FreeDays { get; set; }

        internal bool Validate()
        {
            if (Id.IsEmpty()) throw new ArgumentNullException(nameof(Id));

            if (Platform.IsEmpty()) throw new ArgumentNullException(nameof(Platform));

            if (Title.IsEmpty()) throw new ArgumentNullException(nameof(Title));

            if (Promo.IsEmpty()) throw new ArgumentNullException(nameof(Promo));

            return true;
        }
    }
}
