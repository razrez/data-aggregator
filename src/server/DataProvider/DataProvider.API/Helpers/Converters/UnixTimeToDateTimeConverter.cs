using System.Text.Json.Serialization;
using System.Text.Json;

namespace DataProvider.API.Helpers.Converters;

public class UnixTimeToDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            // Если в JSON число (например, 1719151380)
            long unixTime = reader.GetInt64();
            return DateTimeOffset.FromUnixTimeSeconds(unixTime).UtcDateTime;
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            // Если в JSON строка "1719151380"
            var s = reader.GetString();
            if (long.TryParse(s, out long parsed))
            {
                return DateTimeOffset.FromUnixTimeSeconds(parsed).UtcDateTime;
            }
            return DateTime.MinValue;
        }
        // Если что-то другое — null, объект и т. д.
        return DateTime.MinValue;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // При записи обратно в JSON — отдаем число UnixTime
        long unixTime = ((DateTimeOffset)value).ToUnixTimeSeconds();
        writer.WriteNumberValue(unixTime);
    }
}