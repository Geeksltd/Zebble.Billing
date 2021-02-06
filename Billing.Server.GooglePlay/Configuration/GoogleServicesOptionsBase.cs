namespace Zebble.Billing
{
    using System;
    using Olive;

    public abstract class GoogleServicesOptionsBase
    {
        public string ProjectId { get; set; }
        public string PrivateKeyId { get; set; }
        public string PrivateKey { get; set; }
        public string ClientEmail { get; set; }
        public string ClientId { get; set; }

        protected virtual bool Validate()
        {
            if (ProjectId.IsEmpty()) throw new ArgumentNullException(nameof(ProjectId));

            if (PrivateKeyId.IsEmpty()) throw new ArgumentNullException(nameof(PrivateKeyId));

            if (PrivateKey.IsEmpty()) throw new ArgumentNullException(nameof(PrivateKey));

            if (ClientEmail.IsEmpty()) throw new ArgumentNullException(nameof(ClientEmail));

            if (ClientId.IsEmpty()) throw new ArgumentNullException(nameof(ClientId));

            return true;
        }
    }
}
