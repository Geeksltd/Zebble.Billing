using System.Text.Json.Serialization;
using System.Text.Json;
using System;

namespace Zebble.Billing;

class TimestampToDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetInt64();
        return DateTimeOffset.FromUnixTimeMilliseconds(value);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }
}