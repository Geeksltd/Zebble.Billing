namespace Zebble.Billing
{
    using System.Text.Json.Serialization;

    public enum AppStoreEnvironment
    {
        [JsonPropertyName("Sandbox")]
        Sandbox,

        [JsonPropertyName("Production")]
        Production
    }
}
