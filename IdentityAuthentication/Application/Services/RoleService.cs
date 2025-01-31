using Microsoft.AspNetCore.Identity;
using WebApplication5.Application.Interfaces;
using WebApplication5.Core.Entities;

namespace WebApplication5.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RoleService(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> CreateRoleAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return IdentityResult.Failed(new IdentityError { Description = "Имя роли не может быть пустым." });

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (roleExists)
                return IdentityResult.Failed(new IdentityError { Description = "Такая роль уже существует." });

            return await _roleManager.CreateAsync(new ApplicationRole { Name = roleName });
        }

        public async Task<IdentityResult> DeleteRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return IdentityResult.Failed(new IdentityError { Description = "Роль не найдена." });

            return await _roleManager.DeleteAsync(role);
        }

        public async Task<IEnumerable<string>> GetAllRolesAsync()
        {
            return await Task.FromResult(_roleManager.Roles.Select(r => r.Name).ToList());
        }
    }
}
