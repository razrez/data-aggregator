namespace DataProvider.API.Persistence.Data.Entities;

/// <summary>
/// Сущность для хранения постов в БД (EF).
/// Отличается от Web-модели Post тем, 
/// что мы сами контролируем тип данных для поля Date.
/// </summary>
public class PostEntity
{
  public int Id { get; set; }

  /// <summary>
  /// В модели Post это GroupName (public_name).
  /// </summary>
  public string GroupName { get; set; } = "";

  /// <summary>
  /// Храним в БД DateTime (можно хранить как DateTimeOffset). 
  /// Или если хотите оставлять UnixTime, делаем long DateUnix.
  /// </summary>
  public DateTime Date { get; set; }

  public string Text { get; set; } = "";
  public int Likes { get; set; }
  public int Views { get; set; }
}
