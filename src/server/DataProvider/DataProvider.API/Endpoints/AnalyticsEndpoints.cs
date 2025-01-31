using DataProvider.API.Services;
using System.Text.RegularExpressions;

namespace DataProvider.API.Endpoints;

public static class AnalyticsEndpoints
{
  /// <summary>
  /// Регистрирует эндпоинты для получения статистики.
  /// </summary>
  public static void MapAnalyticsEndpoints(this IEndpointRouteBuilder routes)
  {
    // GET: /api/analytics?startDate=2025-01-01&endDate=2025-01-31
    routes.MapGet("/api/analytics", async (DateTime? startDate, DateTime? endDate, IInfinispanService infinispanService) =>
    {
      var sDate = startDate ?? DateTime.UtcNow.AddYears(-1);
      var eDate = endDate ?? DateTime.UtcNow;

      var keys = await infinispanService.GetAllKeys();
      var hashtagCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

      foreach (var key in keys)
      {
        var post = await infinispanService.GetPostByKey(key);
        if (post == null) continue;

        if (post.Date >= sDate && post.Date <= eDate)
        {
          var matches = Regex.Matches(post.Text, @"\B#\w+");
          foreach (Match match in matches)
          {
            var hashtag = match.Value;
            if (!hashtagCounts.ContainsKey(hashtag))
            {
              hashtagCounts[hashtag] = 0;
            }
            hashtagCounts[hashtag]++;
          }
        }
      }

      return Results.Ok(hashtagCounts);

    })
    .WithName("GetHashtagAnalytics");
    //.WithOpenApi();
  }
}
