using DataProvider.API.Helpers.Converters;
using System.Text.Json.Serialization;

namespace DataProvider.API.Models;

public class Post
{
  [JsonPropertyName("id")]
  public int Id { get; set; }

  // Через кастомный конвертер будет читаться/писаться как UnixTime
  [JsonConverter(typeof(UnixTimeToDateTimeConverter))]
  [JsonPropertyName("date")]
  public DateTime Date { get; set; }

  [JsonPropertyName("text")]
  public string Text { get; set; } = "";

  [JsonPropertyName("likes")]
  public int Likes { get; set; }

  [JsonPropertyName("views")]
  public int Views { get; set; }
}
