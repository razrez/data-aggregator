using DataProvider.API.Models;
using DataProvider.API.Models.DTOs;
using DataProvider.API.Persistence;
using DataProvider.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataProvider.API.Endpoints;

public static class PostEndpoints
{
  /// <summary>
  /// Регистрирует эндпоинты, связанные с постами.
  /// </summary>
  public static void MapPostEndpoints(this IEndpointRouteBuilder routes)
  {
    routes.MapGet("/api/post/", GetPostAsync)
    .WithDescription("Получить пост (look-aside cache: сначала Infinispan, потом БД)")
    .RequireAuthorization();

    // Для получение новой страницы просто увеличиваем offset += limit
    routes.MapGet("/api/posts", GetAllPostsAsync)
    .WithDescription("Получение постов с пагинацией")
    .RequireAuthorization();
  }

  public async static Task<IResult> GetPostAsync(
    [FromQuery] int id,
    IInfinispanService infinispan,
    AppDbContext db)
  {
    // 1. Сначала проверяем Infinispan
    var postFromCache = await infinispan.GetPostByKey(id);
    if (postFromCache != null)
    {
      // Нашли в кэше — просто возвращаем
      return Results.Ok(postFromCache);
    }

    // 2. Если в кэше нет, ищем в БД
    var entity = await db.Posts.FindAsync(id);
    if (entity is null)
    {
      // В БД не нашли => 404
      return Results.NotFound($"No post with id={id}");
    }

    // 3. Перекладываем из БД в модель Post (чтобы в кэше хранился тот же формат)
    var post = new Post
    {
      Id = entity.Id,
      GroupName = entity.GroupName,
      Date = entity.Date,
      Text = entity.Text,
      Likes = entity.Likes,
      Views = entity.Views
    };

    // 4. Записываем в Infinispan (горячая запись)
    await infinispan.PutPostByKey(post.Id, post);

    return TypedResults.Ok(post);
  }

  public static async Task<IResult> GetAllPostsAsync(
    [AsParameters] GetPostsRequest req,
    AppDbContext db, 
    IInfinispanService infinispanService)
  {
    // 1) Защитные проверки
    var limit = req.Limit < 0 ? 10 : req.Limit;
    var offset = req.Offset <= 0 ? 0 : req.Offset;

    // Из базы данных берём *только* Id (и поля для сортировки).
    // Допустим, сортируем по дате убыванию.
    var idsQuery = db.Posts
      .OrderByDescending(p => p.Date)
      .Skip(offset)
      .Take(limit)
      .Select(p => p.Id);

    var postIds = await idsQuery.ToListAsync();
    if (postIds.Count == 0) return TypedResults.NotFound();

    var tasks = postIds.Select(async id => {
      var cached = await infinispanService.GetPostByKey(id);
      if (cached != null) return cached;

      // Если в кеше нет, то берем из БД и кешируем
      var entity = await db.Posts.FindAsync(id);
      if (entity == null) return null;

      var post = new Post
      {
        Id = id,
        Date = entity.Date,
        GroupName = entity.GroupName,
        Likes = entity.Likes,
        Views = entity.Views,
        Text = entity.Text
      };

      await infinispanService.PutPostByKey(id, post);
      return post;
    });

    var pagedPosts = await Task.WhenAll(tasks);
    return TypedResults.Ok(pagedPosts.Where(p => p != null).ToList());
  }

}