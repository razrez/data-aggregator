var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<InfinispanSettings>(builder.Configuration.GetSection("InfinispanSettings"));

var app = builder.Build();

// Получение настроек Infinispan
var infinispanSettings = app.Services.GetRequiredService<IConfiguration>().GetSection("InfinispanSettings").Get<InfinispanSettings>();

HttpClient httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{infinispanSettings.User}:{infinispanSettings.Password}")));

app.MapGet("/get/{key}", async (string key) =>
{
    var response = await httpClient.GetAsync($"{infinispanSettings.Url}/rest/v2/caches/cache-name/{key}");
    if (response.IsSuccessStatusCode)
    {
        var value = await response.Content.ReadAsStringAsync();
        return Results.Ok(new { Key = key, Value = value });
    }
    else
    {
        return Results.NotFound();
    }
});

app.MapGet("/list", async (int offset = 0, int limit = 10) =>
{
    var response =
        await httpClient.GetAsync($"{infinispanSettings.Url}/rest/v2/caches/cache-name?max={limit}&start={offset}");
    if (response.IsSuccessStatusCode)
    {
        var values = await response.Content.ReadFromJsonAsync<List<string>>();
        return Results.Ok(values);
    }
    else
    {
        return Results.NotFound();
    }
});

app.Run();

// Модель для настроек Infinispan
public class InfinispanSettings
{
    public string Url { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
}