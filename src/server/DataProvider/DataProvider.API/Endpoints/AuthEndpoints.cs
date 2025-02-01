using System.Security.Claims;
using DataProvider.API.Models;
using DataProvider.API.Services;

namespace DataProvider.API.Endpoints;

public static class AuthEndpoints
{
    /// <summary>
    /// Регистрирует эндпоинты, связанные с авторизацией
    /// </summary>
    public static void MapAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/api/auth/signin", (User user, JwtService jwtService) =>
        {
            if (user is null)
            {
                return Results.Unauthorized();
            }

            // Генерация токена
            string token = jwtService.GenerateToken(user.Username);

            // Возврат токена в ответе
            return Results.Ok(new { Token = token, ExperationDate = DateTime.Now + TimeSpan.FromMinutes(60) });
        });

        routes.MapGet("/api/auth/user/name", (HttpContext context) =>
        {
            // Проверяем, есть ли пользователь в контексте
            var user = context.User;

            if (user.Identity?.IsAuthenticated != true)
            {
                return Results.Unauthorized(); // Если пользователь не аутентифицирован
            }

            // Извлекаем имя пользователя из claims
            var username = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(username))
            {
                return Results.Problem("Имя пользователя не найдено в токене.", statusCode: 400);
            }

            // Возвращаем имя пользователя
            return Results.Ok(username);
        }).RequireAuthorization();
    }
}