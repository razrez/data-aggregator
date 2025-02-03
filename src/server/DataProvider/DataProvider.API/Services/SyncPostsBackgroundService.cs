using DataProvider.API.Models;
using DataProvider.API.Persistence.Data.Entities;
using DataProvider.API.Persistence;
using DataProvider.API.Helpers.Extensions;

namespace DataProvider.API.Services;

public class SyncPostsBackgroundService : BackgroundService
{
  private readonly IServiceProvider _serviceProvider;
  private readonly ILogger<SyncPostsBackgroundService> _logger;

  public SyncPostsBackgroundService(IServiceProvider serviceProvider, ILogger<SyncPostsBackgroundService> logger)
  {
    _serviceProvider = serviceProvider;
    _logger = logger;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    // Периодически, пока приложение не остановят
    while (!stoppingToken.IsCancellationRequested)
    {
      _logger.LogInformation("Запуск синхронизации постов с Infinispan...");

      try
      {
        using var scope = _serviceProvider.CreateScope();
        var infinispanService = scope.ServiceProvider.GetRequiredService<IInfinispanService>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // 2. Загружаем посты
        var allPosts = await infinispanService.LoadAllPostsAsync();

        // 3. Сохраняем в БД (PostEntity)
        foreach (var p in allPosts)
        {
          // Проверяем, есть ли такой пост уже
          var existing = await db.Posts.FindAsync(p.Id);
          if (existing == null)
          {
            // Новый пост
            var entity = new PostEntity
            {
              Id = p.Id,
              Date = p.Date, // UnixTime
              Text = p.Text,
              Likes = p.Likes,
              Views = p.Views,
              GroupName = p.GroupName,
            };
            await db.Posts.AddAsync(entity, stoppingToken).ConfigureAwait(false);

          }
          else
          {
            // Обновляем поля
            existing.Text = p.Text;
            existing.Date = p.Date;
            existing.Likes = p.Likes;
            existing.Views = p.Views;
            existing.GroupName = p.GroupName;
          }
        }

        await db.SaveChangesAsync().ConfigureAwait(false);
        scope.Dispose();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Ошибка при синхронизации постов.");
      }

      // Ждём минуту перед следующим проходом
      await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
    }
  }
}
