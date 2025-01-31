using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication5.Application.Dto;
using WebApplication5.Application.Interfaces;

namespace WebApplication5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            var result = await _userService.RegisterAsync(model);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User registered successfully!");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var result = await _userService.LoginAsync(model);
            if (result.Succeeded)
            {
                //Отправляем токены в Cookie, чтобы при обновлении страницы не разлогиниться
                SetTokenCookies(result.Response.AccessToken, result.Response.RefreshToken);

                return Ok(result.Response);
            }
            return Unauthorized(result.Error);
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _userService.LogoutAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (Request.Cookies["accessToken"] != null)
            {
                Response.Cookies.Delete("accessToken");
            }

            if (Request.Cookies["refreshToken"] != null)
            {
                Response.Cookies.Delete("refreshToken");
            }
            return Ok("Logged out successfully.");
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var result = await _userService.RefreshTokenAsync(refreshToken);
            if (result.Succeeded)
            {
                //Отправляем токены в Cookie, чтобы при обновлении страницы не разлогиниться
                SetTokenCookies(result.Response.AccessToken, result.Response.RefreshToken);
                return Ok(result.Response);
            }
            return Unauthorized(result.Error);
        }

        private void SetTokenCookies(string accessToken, string refreshToken)
        {
            var accessTokenOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Только для HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(15)
            };

            var refreshTokenOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("accessToken", accessToken, accessTokenOptions);
            Response.Cookies.Append("refreshToken", refreshToken, refreshTokenOptions);
        }
    }
}
