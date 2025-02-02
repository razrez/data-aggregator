using DataProvider.API.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace DataProvider.API.Services;

public class InfinispanService(HttpClient httpClient, IOptions<InfinispanSettings> options) : IInfinispanService
{
  private readonly HttpClient _httpClient = httpClient;
  private readonly InfinispanSettings _settings = options.Value;

  // Получение постов с псевдопагинацией
  public async Task<List<long>> GetAllKeys(int offset, int limit)
  {
    // Вместо ?start={start}&max={max}
    var url = $"{_settings.Url}/rest/v2/caches/{_settings.CacheName}?action=keys";

    var response = await _httpClient.GetAsync(url);

    if (!response.IsSuccessStatusCode)
      return [];

    var stringKeys = await response.Content.ReadFromJsonAsync<List<long>>();

    // «Обрезаем» в коде
    return stringKeys?
      .Skip(offset) // «пропустить <offset> элементов в отсортированном списке»
      .Take(limit)
      .ToList() ?? [];
  }

  public async Task<Post?> GetPostByKey(long key)
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

  public async Task PutPostByKey(long key, Post post)
  {
    var url = $"{_settings.Url}/rest/v2/caches/{_settings.CacheName}/{key}";
    // Для Infinispan, если ключ не существует, POST vs PUT может отличаться, 
    // но чаще PUT создаёт/заменяет.
    
    await _httpClient.PutAsJsonAsync(url, post).ConfigureAwait(false);
  }
}
