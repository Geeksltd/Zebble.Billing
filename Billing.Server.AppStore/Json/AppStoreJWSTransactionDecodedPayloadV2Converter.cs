using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JWT.Builder;

namespace Zebble.Billing;

class AppStoreJWSTransactionDecodedPayloadV2Converter : JsonConverter<AppStoreJWSTransactionDecodedPayloadV2>
{
    static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters = { new TimestampToDateTimeOffsetConverter() }
    };

    static readonly JwtBuilder JwtReader = JwtBuilder.Create().DoNotVerifySignature();

    public override AppStoreJWSTransactionDecodedPayloadV2? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var rawJws = reader.GetString();
        var payloadJson = JwtReader.Decode(rawJws);
        var payload = JsonSerializer.Deserialize<AppStoreJWSTransactionDecodedPayloadV2>(payloadJson, JsonSerializerOptions);
        return payload;
    }

    public override void Write(Utf8JsonWriter writer, AppStoreJWSTransactionDecodedPayloadV2 value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}