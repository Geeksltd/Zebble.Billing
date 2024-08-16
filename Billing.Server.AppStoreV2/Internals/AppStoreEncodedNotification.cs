using System.Text.Json;
using System.Text.Json.Serialization;
using JWT.Builder;

namespace Zebble.Billing;

record AppStoreEncodedNotification([property: JsonPropertyName("signedPayload")] string SignedPayload)
{
    static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    static readonly JwtBuilder JwtReader = JwtBuilder.Create().DoNotVerifySignature();

    public AppStoreDecodedNotification Decode()
    {
        var rawPayload = JwtReader.Decode(SignedPayload);
        var payload = JsonSerializer.Deserialize<AppStoreDecodedNotification>(rawPayload, SerializerOptions);
        return payload!;
    }
}