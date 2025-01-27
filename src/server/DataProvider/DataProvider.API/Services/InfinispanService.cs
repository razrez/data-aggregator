using DataProvider.API.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace DataProvider.API.Services;

public class InfinispanService : IInfinispanService
{
  private readonly HttpClient _httpClient;
  private readonly InfinispanSettings _settings;

  public InfinispanService(HttpClient httpClient, InfinispanSettings settings)
  {
    _httpClient = httpClient;
    _settings = settings;

    // Устанавливаем Basic Auth
    var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{_settings.User}:{_settings.Password}"));
    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
  }

  public async Task<List<string>> GetAllKeys(int start = 0, int max = 999999)
  {
    var url = $"{_settings.Url}/rest/v2/caches/{_settings.CacheName}?start={start}&max={max}";
    var response = await _httpClient.GetAsync(url);

    if (!response.IsSuccessStatusCode)
      return new List<string>();

    var keys = await response.Content.ReadFromJsonAsync<List<string>>();
    return keys ?? new List<string>();
  }

  public async Task<Post?> GetPostByKey(string key)
  {
    var url = $"{_settings.Url}/rest/v2/caches/{_settings.CacheName}/{key}";
    var response = await _httpClient.GetAsync(url);

    if (!response.IsSuccessStatusCode)
      return null;

    var json = await response.Content.ReadAsStringAsync();
    if (string.IsNullOrEmpty(json))
      return null;

    // Тут мы можем десериализовать сразу в словарь, чтобы корректно 
    // обработать UnixTimestamp -> DateTime
    try
    {
      var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
      if (dict == null) return null;

      var post = new Post
      {
        Id = dict["id"].GetInt32(),
        Text = dict["text"].GetString() ?? "",
        Likes = dict["likes"].GetInt32(),
        Views = dict["views"].GetInt32(),
      };

      var unixTime = dict["date"].GetInt64();
      post.Date = DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;

      return post;
    }
    catch
    {
      // Логируем/обрабатываем при необходимости
      return null;
    }
  }

}
