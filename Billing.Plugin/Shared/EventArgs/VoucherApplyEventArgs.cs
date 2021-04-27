namespace Zebble.Billing
{
    using System;

    class VoucherApplyEventArgs : EventArgs
    {
        public string Code { get; set; }
    }
}
