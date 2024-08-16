using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JWT.Builder;

namespace Zebble.Billing;

class AppStoreJWSTransactionDecodedPayloadConverter : JsonConverter<AppStoreJWSTransactionDecodedPayload>
{
    static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters = { new TimestampToDateTimeOffsetConverter() }
    };

    static readonly JwtBuilder JwtReader = JwtBuilder.Create().DoNotVerifySignature();

    public override AppStoreJWSTransactionDecodedPayload? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var rawJws = reader.GetString();
        var payloadJson = JwtReader.Decode(rawJws);
        var payload = JsonSerializer.Deserialize<AppStoreJWSTransactionDecodedPayload>(payloadJson, JsonSerializerOptions);
        return payload;
    }

    public override void Write(Utf8JsonWriter writer, AppStoreJWSTransactionDecodedPayload value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}