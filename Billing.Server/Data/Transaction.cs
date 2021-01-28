namespace Zebble.Billing
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Products")]
    public class Product
    {
        public SubscriptionPlatform? Platform { get; set; }
        public ProductType Type { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public int Months { get; set; }
        public string Promo { get; set; }
        public int FreeDays { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
        public bool IsLifetime { get; set; }
        public decimal Price { get; set; }
    }

    [Table("Transactions")]
    public class Transaction
    {
        public Guid TransactionId { get; set; }
        public Guid SubscriptionId { get; set; }

        public SubscriptionPlatform Platform { get; set; }

        public DateTime Date { get; set; }

        public string Details { get; set; }
    }
}
