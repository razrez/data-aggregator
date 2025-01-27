using System.Text.Json.Serialization;

namespace DataProvider.API.Models;

public class Post
{
  [JsonPropertyName("id")]
  public int Id { get; set; }

  // В Python-сервисе хранится как UnixTime, здесь – DateTime
  public DateTime Date { get; set; }

  [JsonPropertyName("text")]
  public string Text { get; set; } = "";

  [JsonPropertyName("likes")]
  public int Likes { get; set; }

  [JsonPropertyName("views")]
  public int Views { get; set; }
}
