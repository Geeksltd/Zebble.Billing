namespace Zebble.Billing;

using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Olive;

class DateTimeConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        Convert(reader.GetInt64());

    public static DateTime? Convert(long? value)
    {
        if (!(value > 0)) return null;
        return DateTime.UnixEpoch.AddMilliseconds(value.Value);
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(((DateTimeOffset?)value)?.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture).Or(""));
    }
}
