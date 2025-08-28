using freelancer_hub_backend.DTO_s;
using freelancer_hub_backend.Services;
using freelancer_hub_backend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace freelancer_hub_backend.Controllers
{
    [Authorize]
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
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(string id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null) return NotFound();

                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            try
            {
                var userId = UserUtils.GetSupabaseUserId(User);
                var user = await _userService.GetCurrentUserAsync(userId);

                if (user == null) return NotFound();

                return Ok(user);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateOrGetUser(UserCreateDto dto)
        {
            try
            {
                var userId = UserUtils.GetSupabaseUserId(User);
                var user = await _userService.CreateOrGetUserAsync(userId, dto);

                // Se o usuário já existia, retorna 200. Se foi criado, retorna 201
                var existingUser = await _userService.GetUserByIdAsync(userId);
                if (existingUser?.CreatedAt == user.CreatedAt)
                {
                    return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
                }

                return Ok(user);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, UserUpdateDto dto)
        {
            try
            {
                var currentUserId = UserUtils.GetSupabaseUserId(User);
                if (currentUserId != id)
                {
                    return Forbid("Você só pode atualizar seu próprio perfil.");
                }

                await _userService.UpdateUserAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var currentUserId = UserUtils.GetSupabaseUserId(User);
                if (currentUserId != id)
                {
                    return Forbid("Você só pode deletar seu próprio perfil.");
                }

                await _userService.DeleteUserAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}