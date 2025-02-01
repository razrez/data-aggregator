using DataProvider.API.Helpers.Extensions;
using DataProvider.API.Models;
using DataProvider.API.Services;

namespace DataProvider.API.Endpoints;

public static class PostEndpoints
{
  /// <summary>
  /// Регистрирует эндпоинты, связанные с постами.
  /// </summary>
  public static void MapPostEndpoints(this IEndpointRouteBuilder routes)
  {
    routes.MapGet("/api/post/get/{key}", async (string key, IInfinispanService infinispan) =>
    {
      Post? post = await infinispan.GetPostByKey(key);
      return TypedResults.Json(post);
    })
    .WithDescription("Получить пост по ключу");

    // Для получение новой страницы просто увеличиваем offset += limit
    routes.MapGet("/api/posts", async (int offset, int limit, IInfinispanService infinispanService ) =>
    {
      // 1) Сначала получаем нужные ключи (offset/limit).
      var keys = await infinispanService.GetAllKeys(offset, limit);

      // Если limit <= 0, сделаем concurrency = 1, чтобы не делить на ноль и не создавать SemaphoreSlim(0).
      var concurrency = (limit > 0) ? limit : 1;
      var allPosts = await infinispanService.LoadAllPostsAsync(concurrency);

      return TypedResults.Ok(allPosts);
    })
    .WithDescription("Получение постов с пагинацией");
  }
}
