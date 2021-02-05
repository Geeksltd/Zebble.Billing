namespace Zebble.Billing
{
    public abstract class GoogleServicesOptionsBase
    {
        public string ProjectId { get; set; }
        public string PrivateKeyId { get; set; }
        public string PrivateKey { get; set; }
        public string ClientEmail { get; set; }
        public string ClientId { get; set; }
    }
}
