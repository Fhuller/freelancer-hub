using freelancer_hub_backend.DTO_s;
using freelancer_hub_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace freelancer_hub_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly FreelancerContext _context;

        public UserController(FreelancerContext context)
        {
            _context = context;
        }

        // GET: api/user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var users = await _context.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/user/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null) return NotFound();

            var dto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };

            return Ok(dto);
        }

        // POST: api/user
        [HttpPost]
        public async Task<ActionResult<UserDto>> Create(UserCreateDto dto)
        {
            var userId = "a7a8003b-6e49-4ac4-ab16-edde844f3d93";
            
            if (userId == null)
            {
                return Unauthorized();
            }

            var existingUser = await _context.Users.FindAsync(userId);

            if (existingUser != null)
            {
                // Já existe, retorna os dados
                return Ok(new UserDto
                {
                    Id = existingUser.Id,
                    Name = existingUser.Name,
                    Email = existingUser.Email,
                    CreatedAt = existingUser.CreatedAt
                });
            }

            var user = new User
            {
                Id = userId,
                Name = dto.Name,
                Email = dto.Email,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, result);
        }

        // PUT: api/user/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UserUpdateDto dto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null) return NotFound();

            user.Name = dto.Name;
            user.Email = dto.Email;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/user/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

}
