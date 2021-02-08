namespace Zebble.Billing
{
    using System;

    public class Voucher
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public TimeSpan Duration { get; set; }

        public string ProductId { get; set; }
        public string UserId { get; set; }

        public DateTime? ActivationDate { get; set; }
    }
}
