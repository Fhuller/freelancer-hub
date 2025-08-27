using freelancer_hub_backend.DTO_s;
using freelancer_hub_backend.Models;
using freelancer_hub_backend.Services;
using freelancer_hub_backend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace freelancer_hub_backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly FreelancerContext _context;
        private readonly IClientService _clientService;

        public ClientController(FreelancerContext context, IClientService clientService)
        {
            _context = context;
            _clientService = clientService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientReadDto>>> GetClients()
        {
            try
            {
                var userId = UserUtils.GetSupabaseUserId(User);
                var clients = await _clientService.GetClientsAsync(userId);
                return Ok(clients);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClientReadDto>> GetClientById(Guid id)
        {
            var client = await _clientService.GetClientByIdAsync(id);
            if (client == null) return NotFound();

            return Ok(client);
        }


        [HttpPost]
        public async Task<ActionResult<ClientReadDto>> CreateClient(ClientCreateDto dto)
        {
            try
            {
                var userId = UserUtils.GetSupabaseUserId(User);
                var client = await _clientService.CreateClientAsync(userId, dto);
                return CreatedAtAction(nameof(GetClientById), new { id = client.Id }, client);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/client/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(Guid id, ClientUpdateDto dto)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
                return NotFound();

            client.Name = dto.Name;
            client.Email = dto.Email;
            client.Phone = dto.Phone;
            client.CompanyName = dto.CompanyName;
            client.Notes = dto.Notes;

            _context.Entry(client).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(Guid id)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
                return NotFound();

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
