namespace Zebble.Billing
{
    using System.Text.Json.Serialization;

    enum AppStoreEnvironment
    {
        [JsonPropertyName("Sandbox")]
        Sandbox,

        [JsonPropertyName("Production")]
        Production
    }
}
