namespace Zebble.Billing
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    class DateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;
            return DateTime.UnixEpoch.AddMilliseconds(reader.GetInt64());
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value is null)
                writer.WriteNullValue();
            else
                writer.WriteNumberValue(((DateTimeOffset)value).ToUnixTimeMilliseconds());
        }
    }
}
