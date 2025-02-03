using DataProvider.API.Models;
using DataProvider.API.Services;

namespace DataProvider.API.Helpers.Extensions;

public static class InfiniSpanServiceExtensions
{
  public static async Task<List<Post>> LoadAllPostsAsync(this IInfinispanService infinispanService, List<long>? keys = null, int concurrency = 10)
  {
    if (keys == null) keys = await infinispanService.GetAllKeys();

    using var semaphore = new SemaphoreSlim(concurrency, concurrency);
    var tasks = new List<Task<Post?>>();

    foreach (var key in keys)
    {
      tasks.Add(Task.Run(async () =>
      {
        await semaphore.WaitAsync();
        try
        {
          return await infinispanService.GetPostByKey(key);
        }
        finally
        { 
          semaphore.Release();
        }
      }));
    }

    var posts = await Task.WhenAll(tasks);
    return [.. posts];
  }
}
