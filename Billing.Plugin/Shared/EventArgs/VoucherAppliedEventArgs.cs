namespace Zebble.Billing
{
    using System;

    public class VoucherAppliedEventArgs : EventArgs
    {
        public string VoucherCode { get; set; }
    }
}
