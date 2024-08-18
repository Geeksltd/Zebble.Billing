using System.Text.Json;
using System.Text.Json.Serialization;
using JWT.Builder;

namespace Zebble.Billing;

record AppStoreEncodedNotificationV2([property: JsonPropertyName("signedPayload")] string SignedPayload)
{
    static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    static readonly JwtBuilder JwtReader = JwtBuilder.Create().DoNotVerifySignature();

    public AppStoreDecodedNotificationV2 Decode()
    {
        var rawPayload = JwtReader.Decode(SignedPayload);
        var payload = JsonSerializer.Deserialize<AppStoreDecodedNotificationV2>(rawPayload, SerializerOptions);
        return payload!;
    }
}