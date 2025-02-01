using DataProvider.API.Endpoints;
using DataProvider.API.Models;
using DataProvider.API.Services;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using DataProvider.API.Startup;
using Microsoft.OpenApi.Models;

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
  .AddSwaggerGen(options =>
  {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
      Description = "JWT Authorization header using the Bearer scheme.",
      Name = "Authorization",
      In = ParameterLocation.Header,
      Type = SecuritySchemeType.ApiKey,
      Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
      {
        new OpenApiSecurityScheme
        {
          Reference = new OpenApiReference
          {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
          }
        },
        new string[] {}
      }
    });
  });

builder.Services.AddCors(options =>
{
  options.AddPolicy("CorsPolicy", builder =>
  {
    builder.AllowAnyOrigin()
      .AllowAnyHeader()
      .AllowAnyMethod();
  });
});

builder.Services.AddAuth(builder.Configuration);

var app = builder.Build();

// Подключаем Swagger
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseAuth();

// 5) Маппим наши эндпоинты
app.MapPostEndpoints();
app.MapAnalyticsEndpoints();
app.MapAuthEndpoints();

app.UseCors("CorsPolicy");

app.Run();