using Microsoft.AspNetCore.Mvc;
using WebApplication5.Application.Dto;
using WebApplication5.Application.Interfaces;

namespace WebApplication5.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UserDTO userDto)
        {
            var result = await _userService.UpdateUserAsync(userDto);
            return result ? Ok("User updated successfully.") : BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteUserAsync(id);

            return result ? Ok("User deleted successfully.") : BadRequest();
        }

        // 1. Назначение роли пользователю
        [HttpPost("add-role")]
        public async Task<IActionResult> AddRoleToUser([FromBody] UserRoleDto model)
        {
            var result = await _userService.AddRoleToUserAsync(model);

            return result ? Ok("Role added to user successfully.") : BadRequest();
        }

        // 2. Удаление роли у пользователя
        [HttpPost("remove-role")]
        public async Task<IActionResult> RemoveRoleFromUser([FromBody] UserRoleDto model)
        {
            var result = await _userService.RemoveRoleFromUserAsync(model);

            return result ? Ok("Role removed from user successfully.") : BadRequest();
        }

        // 3. Получение ролей пользователя
        [HttpGet("{userId}/roles")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var roles = await _userService.GetUserRolesAsync(userId);
            return Ok(roles);
        }
    }
}
