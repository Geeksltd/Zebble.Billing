using System.Text.Json;
using System.Text.Json.Serialization;
using JWT.Builder;
using System;

namespace Zebble.Billing;

class AppStoreJWSRenewalInfoDecodedPayloadV2Converter : JsonConverter<AppStoreJWSRenewalInfoDecodedPayloadV2>
{
    internal static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters = { new TimestampToDateTimeOffsetConverter() }
    };

    static readonly JwtBuilder JwtReader = JwtBuilder.Create().DoNotVerifySignature();

    public override AppStoreJWSRenewalInfoDecodedPayloadV2? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var rawJws = reader.GetString();
        var payloadJson = JwtReader.Decode(rawJws);
        var payload = JsonSerializer.Deserialize<AppStoreJWSRenewalInfoDecodedPayloadV2>(payloadJson, JsonSerializerOptions);
        return payload;
    }

    public override void Write(Utf8JsonWriter writer, AppStoreJWSRenewalInfoDecodedPayloadV2 value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}