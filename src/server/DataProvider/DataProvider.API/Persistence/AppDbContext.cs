using DataProvider.API.Persistence.Data.Configurations;
using DataProvider.API.Persistence.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataProvider.API.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
  public DbSet<PostEntity> Posts { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    // Применяем конфигурацию
    modelBuilder.ApplyConfiguration(new PostEntityConfiguration());

    base.OnModelCreating(modelBuilder);
  }
}
