namespace Zebble.Billing
{
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum HuaweiEnvironment
    {
        [EnumMember(Value = "Sandbox")]
        Sandbox,

        [EnumMember(Value = "Production")]
        Production
    }
}
