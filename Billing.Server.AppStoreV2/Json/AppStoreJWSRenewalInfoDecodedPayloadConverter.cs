using System.Text.Json;
using System.Text.Json.Serialization;
using JWT.Builder;
using System;

namespace Zebble.Billing;

class AppStoreJWSRenewalInfoDecodedPayloadConverter : JsonConverter<AppStoreJWSRenewalInfoDecodedPayload>
{
    internal static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters = { new TimestampToDateTimeOffsetConverter() }
    };

    static readonly JwtBuilder JwtReader = JwtBuilder.Create().DoNotVerifySignature();

    public override AppStoreJWSRenewalInfoDecodedPayload? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var rawJws = reader.GetString();
        var payloadJson = JwtReader.Decode(rawJws);
        var payload = JsonSerializer.Deserialize<AppStoreJWSRenewalInfoDecodedPayload>(payloadJson, JsonSerializerOptions);
        return payload;
    }

    public override void Write(Utf8JsonWriter writer, AppStoreJWSRenewalInfoDecodedPayload value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}