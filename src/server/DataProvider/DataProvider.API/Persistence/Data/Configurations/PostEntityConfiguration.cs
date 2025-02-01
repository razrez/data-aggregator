using DataProvider.API.Persistence.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DataProvider.API.Persistence.Data.Configurations;

public class PostEntityConfiguration : IEntityTypeConfiguration<PostEntity>
{
  public void Configure(EntityTypeBuilder<PostEntity> builder)
  {
    // Название таблицы
    builder.ToTable("posts");

    // PK
    builder.HasKey(x => x.Id);

    // Id задаётся снаружи (не генерируется автоматически)
    builder.Property(x => x.Id)
        .ValueGeneratedNever();

    // Поле GroupName => group_name (varchar(200) условно)
    builder.Property(x => x.GroupName)
        .HasColumnName("group_name")
        .HasMaxLength(200)
        .IsRequired();

    // Поле Date
    builder.Property(x => x.Date)
        .HasColumnName("date")
        // Для PostgreSQL под капотом обычно будет TIMESTAMP (без временной зоны),
        // или TIMESTAMP WITH TIME ZONE, если в UseNpgsql(...) так настроено
        .IsRequired();

    builder.Property(x => x.Text)
        .HasColumnName("text");

    builder.Property(x => x.Likes)
        .HasColumnName("likes");

    builder.Property(x => x.Views)
        .HasColumnName("views");
  }
}