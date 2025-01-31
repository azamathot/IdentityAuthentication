using Microsoft.AspNetCore.Mvc;
using WebApplication5.Application.Interfaces;

namespace WebApplication5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // 1. Создание роли
        [HttpPost("create")]
        public async Task<IActionResult> CreateRoleAsync([FromBody] string roleName)
        {
            var result = await _roleService.CreateRoleAsync(roleName);
            if (result.Succeeded)
                return Ok($"Роль '{roleName}' успешно создана.");

            return BadRequest(result.Errors);
        }

        // 2. Удаление роли
        [HttpDelete("{roleName}")]
        public async Task<IActionResult> DeleteRoleAsync(string roleName)
        {
            var result = await _roleService.DeleteRoleAsync(roleName);
            if (result.Succeeded)
                return Ok($"Роль '{roleName}' успешно удалена.");

            return BadRequest(result.Errors);
        }

        // 3. Получение всех ролей
        [HttpGet("all")]
        public async Task<IActionResult> GetAllRolesAsync()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }
    }
}
