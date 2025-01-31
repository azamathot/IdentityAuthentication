using Microsoft.AspNetCore.Identity;

namespace WebApplication5.Application.Interfaces
{
    public interface IRoleService
    {
        Task<IdentityResult> CreateRoleAsync(string roleName);
        Task<IdentityResult> DeleteRoleAsync(string roleName);
        Task<IEnumerable<string>> GetAllRolesAsync();
    }
}
