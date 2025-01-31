using DataProvider.API.Endpoints;
using DataProvider.API.Models;
using DataProvider.API.Services;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization.Metadata;

var builder = WebApplication.CreateBuilder(args);

// Конфигурируем InfinispanSettings
builder.Services
  .Configure<InfinispanSettings>(builder.Configuration.GetSection("InfinispanSettings"))
  .Configure<JsonOptions>(options =>
  {
    options.SerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver();
  });

builder.Services
  .AddHttpClient<IInfinispanService, InfinispanService>()
  .ConfigureHttpClient((provider, client) =>
  {
    // Берём настройки из IOptions<InfinispanSettings>
    var options = provider.GetRequiredService<IOptions<InfinispanSettings>>();
    var settings = options.Value;

    var token = Convert.ToBase64String(
      Encoding.UTF8.GetBytes($"{settings.User}:{settings.Password}")
    );
    client.DefaultRequestHeaders.Authorization =
      new AuthenticationHeaderValue("Basic", token);

    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(
      new MediaTypeWithQualityHeaderValue("application/json")
    );
  });

// Swagger для тестирования
builder.Services
  .AddEndpointsApiExplorer()
  .AddSwaggerGen();

var app = builder.Build();

// Подключаем Swagger
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

// 5) Маппим наши эндпоинты
app.MapPostEndpoints();
app.MapAnalyticsEndpoints();

app.Run();