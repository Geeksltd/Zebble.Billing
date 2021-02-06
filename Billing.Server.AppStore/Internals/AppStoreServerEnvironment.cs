namespace Zebble.Billing
{
    using System.Text.Json.Serialization;

    enum AppStoreServerEnvironment
    {
        [JsonPropertyName("Sandbox")]
        SandBox,

        [JsonPropertyName("PROD")]
        Production
    }
}
