using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OSItemIndex.Data.Extensions
{
    public class UnixDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64()).DateTime;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
