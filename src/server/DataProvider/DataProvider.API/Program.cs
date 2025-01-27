using DataProvider.API.Endpoints;
using DataProvider.API.Models;
using DataProvider.API.Services;

var builder = WebApplication.CreateBuilder(args);

// 1) Конфигурируем и считываем InfinispanSettings
builder.Services.Configure<InfinispanSettings>(builder.Configuration.GetSection("InfinispanSettings"));

// 2) Регистрируем HttpClient (Singleton)
builder.Services.AddSingleton<HttpClient>();

// 3) Регистрируем наш сервис InfinispanService в DI
builder.Services.AddSingleton<IInfinispanService>(provider =>
{
  var httpClient = provider.GetRequiredService<HttpClient>();
  var infinispanSettings = provider
    .GetRequiredService<IConfiguration>()
    .GetSection("InfinispanSettings")
    .Get<InfinispanSettings>() ?? new InfinispanSettings();

  return new InfinispanService(httpClient, infinispanSettings);
});

// 4) Swagger для тестирования
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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