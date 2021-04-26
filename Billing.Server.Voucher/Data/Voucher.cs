namespace Zebble.Billing
{
    using System;

    public class Voucher
    {
        public virtual string Id { get; set; }

        public virtual string Code { get; set; }
        public TimeSpan Duration { get; set; }
        public string ProductId { get; set; }
        public string Comments { get; set; }

        public virtual string UserId { get; set; }
        public DateTime? ActivationDate { get; set; }
    }
}
