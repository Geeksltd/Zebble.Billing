namespace Zebble.Billing
{
    using System.Text.Json;

    public static class JsonExtensions
    {
        public static string ToJson(this object value)
        {
            return JsonSerializer.Serialize(value);
        }

        public static T FromJson<T>(this string value)
        {
            return JsonSerializer.Deserialize<T>(value);
        }
    }
}
