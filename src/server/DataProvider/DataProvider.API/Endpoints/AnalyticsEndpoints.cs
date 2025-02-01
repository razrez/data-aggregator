using DataProvider.API.Models;
using DataProvider.API.Persistence;
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
    // 1) Получить все посты за указанный период
    // curl -X GET http://localhost:5163/api/analytics?startDate=2024-02-24&endDate=2024-02-28
    routes.MapGet("/api/analytics", async (DateTime? startDate, DateTime? endDate, IInfinispanService infinispanService, AppDbContext db) =>
    {

      // Если не заданы startDate/endDate, берём «дефолтные» (например, последний год)
      var sDate = startDate ?? DateTime.UtcNow.AddYears(-1);
      var eDate = endDate ?? DateTime.UtcNow;

      // Получаем все ключи
      var keys = await infinispanService.GetAllKeys();
      var allPosts = new List<Post>();

      // Загружаем все посты (фильтрация ниже)
      foreach (var key in keys)
      {
        var post = await infinispanService.GetPostByKey(key);
        if (post != null) allPosts.Add(post);
      }

      // Фильтруем по дате
      var filteredPosts = allPosts
        .Where(p => p.Date >= sDate && p.Date <= eDate)
        .OrderByDescending(p => p.Date) // допустим, сортируем по дате убывания
        .ToList();

      return Results.Ok(filteredPosts);
    });

    // 2) Получить топ N постов по лайкам за период
    // curl -X GET http://localhost:5163/api/analytics/topLiked?startDate=2024-02-24&endDate=2024-03-24&count=10
    routes.MapGet("/api/analytics/topLiked", async (DateTime? startDate, DateTime? endDate, int count, AppDbContext db) =>
    {
      var sDate = startDate ?? DateTime.UtcNow.AddYears(-1);
      var eDate = endDate ?? DateTime.UtcNow;
      var topCount = (count <= 0) ? 10 : count;

      var query = db.Posts
          .Where(p => p.Date >= sDate && p.Date <= eDate)
          .OrderByDescending(p => p.Likes)
          .Take(topCount);

      var topLiked = await query.ToListAsync();

      return TypedResults.Ok(topLiked);
    });
  }
}
