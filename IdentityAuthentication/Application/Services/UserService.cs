using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication5.Application.Dto;
using WebApplication5.Application.Dto.Response;
using WebApplication5.Application.Interfaces;

namespace WebApplication5.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenService _jwtTokenService; // Сервис для работы с токенами
        private readonly IConfiguration _configuration;

        public UserService(UserManager<ApplicationUser> userManager, IJwtTokenService tokenService, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = tokenService;
            _configuration = configuration;
        }

        // Получить список пользователей
        public async Task<IEnumerable<UserDTO>> GetUsersAsync()
        {
            var users = await _userManager.Users.ToArrayAsync();
            return users.Select(u => new UserDTO
            {
                Id = u.Id,
                Email = u.Email,
                UserName = u.UserName
            });
        }

        // Получить пользователя по ID
        public async Task<UserDTO> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            return new UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName
            };
        }

        // Обновить данные пользователя
        public async Task<bool> UpdateUserAsync(UserDTO userDto)
        {
            var user = await _userManager.FindByIdAsync(userDto.Id.ToString());
            if (user == null)
            {
                return false;
            }

            user.UserName = userDto.UserName;
            user.Email = userDto.Email;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        // Удалить пользователя
        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        // Добавить роль пользователю
        public async Task<bool> AddRoleToUserAsync(UserRoleDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.AddToRoleAsync(user, model.RoleName);
            return result.Succeeded;
        }

        // Удалить роль у пользователя
        public async Task<bool> RemoveRoleFromUserAsync(UserRoleDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);
            return result.Succeeded;
        }

        // Получить роли пользователя
        public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Enumerable.Empty<string>();
            }

            return await _userManager.GetRolesAsync(user);
        }

        // Регистрация пользователя
        public async Task<IdentityResult> RegisterAsync(RegisterDTO model)
        {
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            return result;
        }

        // Логин пользователя с возвращением токенов
        public async Task<AuthResult> LoginAsync(LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Login) ??
                await _userManager.FindByNameAsync(model.Login);
            if (user == null)
                return AuthResult.Failure("Invalid credentials");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return AuthResult.Failure("Invalid credentials");

            var accessToken = await _jwtTokenService.GenerateAccessTokenAsync(user); // Генерация access токена
            var refreshToken = _jwtTokenService.GenerateRefreshToken(); // Генерация refresh токена

            // Сохраняем RefreshToken в БД
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpiryDays"]));
            await _userManager.UpdateAsync(user);

            var authResponse = new AuthResponseDTO(accessToken, refreshToken);
            return AuthResult.Success(authResponse);
        }

        public async Task<bool> LogoutAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.RefreshToken = null;
                await _userManager.UpdateAsync(user);
                await _userManager.UpdateSecurityStampAsync(user);
            }

            return true; // Просто для примера
        }

        public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
        {
            // Получаем пользователя из refresh token
            var principal = _jwtTokenService.GetPrincipalFromExpiredToken(refreshToken);
            var userId = principal?.Identity?.Name;
            if (userId == null)
            {
                return AuthResult.Failure("Invalid refresh token");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return AuthResult.Failure("User not found");
            }

            // Проверяем refresh token в базе
            if (user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime > DateTime.UtcNow)
            {
                return AuthResult.Failure("Invalid refresh token");
            }

            // Генерация нового access и refresh token
            var newAccessToken = await _jwtTokenService.GenerateAccessTokenAsync(user);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

            // Сохраняем новый refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpiryDays"]));
            await _userManager.UpdateAsync(user);

            return AuthResult.Success(new AuthResponseDTO(newAccessToken, newRefreshToken));
        }
    }
}
