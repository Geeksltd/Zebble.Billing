namespace Zebble.Billing
{
    public class ApplyVoucherResult
    {
        public VoucherApplyStatus Status { get; set; }

        internal static ApplyVoucherResult From(VoucherApplyStatus status) => new() { Status = status };

        internal static ApplyVoucherResult Succeeded() => new() { Status = VoucherApplyStatus.Succeeded };
    }
}
