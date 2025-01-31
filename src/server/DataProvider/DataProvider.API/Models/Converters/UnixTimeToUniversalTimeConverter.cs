using System.Text.Json.Serialization;
using System.Text.Json;

namespace DataProvider.API.Models.Converters;

public class UnixTimeToUniversalTimeConverter : JsonConverter<DateTime>
{
  public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    // Предполагаем, что в JSON лежит целое число с UnixTime
    var unixTime = reader.GetInt64();
    return DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
  }

  public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
  {
    // Сериализуем в виде ISO8601
    // Пример: 2025-01-27T12:34:56Z
    writer.WriteStringValue(value.ToUniversalTime().ToString("O"));
  }
}