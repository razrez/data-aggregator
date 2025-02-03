using DataProvider.API.Models;
using DataProvider.API.Persistence;
using DataProvider.API.Persistence.Data.Entities;
using DataProvider.API.Services;
using Microsoft.EntityFrameworkCore;

namespace DataProvider.API.Endpoints;

public static class AnalyticsEndpoints
{
  /// <summary>
  /// Регистрирует эндпоинты для получения статистики.
  /// </summary>
  public static void MapAnalyticsEndpoints(this IEndpointRouteBuilder routes)
  {
    // curl -X GET "http://localhost:5163/api/analytics?startDate=2025-01-01T00:00:00Z&endDate=2025-01-04T00:00:00Z" \ -H "Accept: application/json"
    routes.MapGet("/api/analytics", GetTimeFilteredPosts)
      .WithDescription("Получить все посты за указанный период");

    // curl -X GET "http://localhost:5163/api/analytics/topLiked?startDate=2025-01-01T00:00:00Z&endDate=2025-01-14T00:00:00Z&count=5&sortBy=views" \ -H "Accept: application/json"
    // curl -X GET "http://localhost:5163/api/analytics/topPosts?startDate=2025-01-01T00:00:00Z&endDate=2025-01-31T23:59:59Z&count=10&sortBy=likes" \ -H "Accept: application/json"
    routes.MapGet("/api/analytics/topLiked", GetTopPosts)
      .WithDescription("Получить топ N постов по likes/views за период");
  }

  public static async Task<IResult> GetTimeFilteredPosts(DateTime? startDate, DateTime? endDate, IInfinispanService infinispanService, IServiceProvider serviceProvider)
  {
    var sDate = DateTime.SpecifyKind(startDate ?? DateTime.UtcNow.AddYears(-1), DateTimeKind.Utc);
    var eDate = DateTime.SpecifyKind(endDate ?? DateTime.UtcNow, DateTimeKind.Utc);

    List<int> postIds;
    Dictionary<int, PostEntity> postEntities;

    // Используем `using` для кратковременного создания `DbContext`
    using (var scope = serviceProvider.CreateScope())
    {
      var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

      postIds = await db.Posts
          .Where(p => DateTime.SpecifyKind(p.Date, DateTimeKind.Utc) >= sDate
             && DateTime.SpecifyKind(p.Date, DateTimeKind.Utc) <= eDate)
          .OrderByDescending(p => p.Date)
          .Select(p => p.Id)
          .ToListAsync();

      if (postIds.Count == 0) return TypedResults.NotFound();

      postEntities = await db.Posts
          .Where(p => postIds.Contains(p.Id))
          .ToDictionaryAsync(p => p.Id);
    }

    var semaphore = new SemaphoreSlim(10);
    var tasks = postIds.Select(async id =>
    {
      await semaphore.WaitAsync();
      try
      {
        var cached = await infinispanService.GetPostByKey(id);
        if (cached != null) return cached;

        if (!postEntities.TryGetValue(id, out var entity)) return null;

        var post = new Post
        {
          Id = entity.Id,
          Date = entity.Date,
          GroupName = entity.GroupName,
          Likes = entity.Likes,
          Views = entity.Views,
          Text = entity.Text
        };

        await infinispanService.PutPostByKey(id, post);
        return post;
      }
      finally
      {
        semaphore.Release();
      }
    });

    var filteredPosts = await Task.WhenAll(tasks);
    return TypedResults.Ok(filteredPosts.Where(p => p != null));
  }

  /// <summary>
  /// Получение топ N постов по лайкам или просмотрам за указанный период.
  /// </summary>
  public static async Task<IResult> GetTopPosts(
      DateTime? startDate,
      DateTime? endDate,
      int count,
      string sortBy,
      IInfinispanService infinispanService,
      IServiceProvider serviceProvider)
  {
    var sDate = DateTime.SpecifyKind(startDate ?? DateTime.UtcNow.AddYears(-1), DateTimeKind.Utc);
    var eDate = DateTime.SpecifyKind(endDate ?? DateTime.UtcNow, DateTimeKind.Utc);
    var topCount = (count <= 0) ? 10 : count;

    if (sortBy != "likes" && sortBy != "views")
    {
      return TypedResults.BadRequest("sortBy должен быть 'likes' или 'views'.");
    }

    List<int> postIds;
    Dictionary<int, PostEntity> postEntities;

    using (var scope = serviceProvider.CreateScope())
    {
      var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

      var query = db.Posts
          .Where(p => p.Date >= sDate && p.Date <= eDate);

      query = sortBy == "likes"
          ? query.OrderByDescending(p => p.Likes)
          : query.OrderByDescending(p => p.Views);

      postIds = await query
          .Take(topCount)
          .Select(p => p.Id)
          .ToListAsync();

      if (postIds.Count == 0) return TypedResults.NotFound();

      postEntities = await db.Posts
          .Where(p => postIds.Contains(p.Id))
          .ToDictionaryAsync(p => p.Id);
    }

    var semaphore = new SemaphoreSlim(10);
    var tasks = postIds.Select(async id =>
    {
      await semaphore.WaitAsync();
      try
      {
        var cached = await infinispanService.GetPostByKey(id);
        if (cached != null) return cached;

        if (!postEntities.TryGetValue(id, out var entity)) return null;

        var post = new Post
        {
          Id = entity.Id,
          Date = entity.Date,
          GroupName = entity.GroupName,
          Likes = entity.Likes,
          Views = entity.Views,
          Text = entity.Text
        };

        await infinispanService.PutPostByKey(id, post);
        return post;
      }
      finally
      {
        semaphore.Release();
      }
    });

    var topPosts = await Task.WhenAll(tasks);
    return TypedResults.Ok(topPosts.Where(p => p != null));
  }
}
