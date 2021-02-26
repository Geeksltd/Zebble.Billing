namespace Zebble.Billing
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    class StringToLongConverter : JsonConverter<long?>
    {
        public override long? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Convert.ToInt64(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, long? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.ToString());
        }
    }
}