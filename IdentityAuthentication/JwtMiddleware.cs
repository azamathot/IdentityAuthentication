using Microsoft.AspNetCore.Identity;
using WebApplication5.Application.Interfaces;

namespace WebApplication5
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task Invoke(HttpContext context, IJwtTokenService jwtService)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                var token = context.Request.Cookies["accessToken"]; //Получаем токен из Cookie
                if (string.IsNullOrEmpty(token))
                {
                    token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last(); //Проверяем заголовок
                }

                if (!string.IsNullOrEmpty(token))
                {
                    var userId = jwtService.ValidateToken(token);
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var user = await userManager.FindByIdAsync(userId);
                        if (user != null)
                        {
                            //Проверяем, не изменился ли SecurityStamp (выход или смена пароля)
                            var securityStampClaim = jwtService.GetSecurityStampFromToken(token);
                            if (securityStampClaim != user.SecurityStamp)
                            {
                                context.Response.Cookies.Delete("accessToken");
                                context.Response.Cookies.Delete("refreshToken");
                                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                await context.Response.WriteAsync("Invalid token");
                                return;
                            }

                            context.Items["User"] = user; //Устанавливаем пользователя в контекст
                        }
                    }
                }
            }

            await _next(context);
        }
    }
}
