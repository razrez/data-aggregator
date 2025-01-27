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
    // 1) Получить пост по ключу
    routes.MapGet("/api/post/get/{key}", async (string key, IInfinispanService infinispan) =>
    {
      var post = await infinispan.GetPostByKey(key);
      return post is not null ? Results.Ok(post) : Results.NotFound();
    })
    .WithName("GetPostByKey");
    //.WithOpenApi(); // только если используете Swashbuckle/Swagger

    // 2) Список последних N постов (сортировка по убыванию даты)
    routes.MapGet("/api/post/list", async (int limit, IInfinispanService infinispan) =>
    {
      var keys = await infinispan.GetAllKeys();
      var posts = new List<Post>();

      // Собираем посты
      foreach (var key in keys)
      {
        var post = await infinispan.GetPostByKey(key);
        if (post != null) posts.Add(post);
      }

      // Сортируем по убыванию даты и берем limit
      posts = posts
          .OrderByDescending(p => p.Date)
          .Take(limit > 0 ? limit : 10)  // если limit не задан, возьмём 10
          .ToList();

      return TypedResults.Ok(posts);
    })
    .WithName("GetLastPosts");
    //.WithOpenApi();
  }
}
