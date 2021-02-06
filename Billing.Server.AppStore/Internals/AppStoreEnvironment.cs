namespace Zebble.Billing
{
    using System.Text.Json.Serialization;

    enum AppStoreEnvironment
    {
        [JsonPropertyName("Sandbox")]
        SandBox,

        [JsonPropertyName("PROD")]
        Production
    }
}
