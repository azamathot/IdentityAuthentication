using Microsoft.AspNetCore.Identity;
using WebApplication5.Application.Dto;
using WebApplication5.Application.Dto.Response;

namespace WebApplication5.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetUsersAsync();
        Task<UserDTO> GetUserByIdAsync(string id);
        Task<bool> UpdateUserAsync(UserDTO user);
        Task<bool> DeleteUserAsync(string id);
        Task<bool> AddRoleToUserAsync(UserRoleDto model);
        Task<bool> RemoveRoleFromUserAsync(UserRoleDto model);
        Task<IEnumerable<string>> GetUserRolesAsync(string userId);
        Task<IdentityResult> RegisterAsync(RegisterDTO model);
        Task<AuthResult> LoginAsync(LoginDTO model);
        Task<bool> LogoutAsync(string userId);
        Task<AuthResult> RefreshTokenAsync(string refreshToken);
    }
}
