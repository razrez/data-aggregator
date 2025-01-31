using DataProvider.API.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace DataProvider.API.Services;

public class InfinispanService(HttpClient httpClient, IOptions<InfinispanSettings> options) : IInfinispanService
{
  private readonly HttpClient _httpClient = httpClient;
  private readonly InfinispanSettings _settings = options.Value;

  // Получение постов с псевдопагинацией
  public async Task<List<string>> GetAllKeys(int start = 0, int max = 999999)
  {
    var url = $"{_settings.Url}/rest/v2/caches/{_settings.CacheName}?start={start}&max={max}";
    var response = await _httpClient.GetAsync(url);

    if (!response.IsSuccessStatusCode)
      return new List<string>();
    
    var json = await response.Content.ReadAsStringAsync();

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
    if (string.IsNullOrEmpty(json)) return null;

    try
    {
      return JsonSerializer.Deserialize<Post>(json);
    }
    catch
    {
      // Логируем/обрабатываем при необходимости
      return null;
    }
  }

}
