using DataProvider.API.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace DataProvider.API.Services;

public class InfinispanService(HttpClient httpClient, IOptions<InfinispanSettings> options) : IInfinispanService
{
  private readonly HttpClient _httpClient = httpClient;
  private readonly InfinispanSettings _settings = options.Value;

  // Получение постов с псевдопагинацией
  public async Task<List<string>> GetAllKeys(int offset, int limit)
  {
    // Вместо ?start={start}&max={max}
    var url = $"{_settings.Url}/rest/v2/caches/{_settings.CacheName}?action=keys";

    var response = await _httpClient.GetAsync(url);

    if (!response.IsSuccessStatusCode)
      return [];

    var numericKeys = await response.Content.ReadFromJsonAsync<List<long>>();
    var allKeys = numericKeys?.Select(x => x.ToString()).ToList() ?? new List<string>();

    // «Обрезаем» в коде
    return allKeys
      .Skip(offset) // «пропустить <offset> элементов в отсортированном списке»
      .Take(limit)
      .ToList() ?? [];
  }

  public async Task<Post?> GetPostByKey(string key)
  {
    var url = $"{_settings.Url}/rest/v2/caches/{_settings.CacheName}/{key}";
    var response = await _httpClient.GetAsync(url);

    if (!response.IsSuccessStatusCode) return null;

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
