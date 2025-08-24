using freelancer_hub_backend.DTO_s;
using freelancer_hub_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace freelancer_hub_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly FreelancerContext _context;

        public ClientController(FreelancerContext context)
        {
            _context = context;
        }

        // GET: api/client
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientReadDto>>> GetClients()
        {
            var clients = await _context.Clients
                .Select(c => new ClientReadDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    Phone = c.Phone,
                    CompanyName = c.CompanyName,
                    Notes = c.Notes,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return Ok(clients);
        }

        // GET: api/client/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientReadDto>> GetClient(Guid id)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
                return NotFound();

            var dto = new ClientReadDto
            {
                Id = client.Id,
                Name = client.Name,
                Email = client.Email,
                Phone = client.Phone,
                CompanyName = client.CompanyName,
                Notes = client.Notes,
                CreatedAt = client.CreatedAt
            };

            return Ok(dto);
        }

        // POST: api/client
        [HttpPost]
        public async Task<ActionResult<ClientReadDto>> CreateClient(ClientCreateDto dto)
        {
            // Supondo que você pega o UserId do token OAuth futuramente.
            var userId = "Guid.NewGuid()"; // Aqui substitua depois pelo UserId autenticado.

            var client = new Client
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                CompanyName = dto.CompanyName,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            var readDto = new ClientReadDto
            {
                Id = client.Id,
                Name = client.Name,
                Email = client.Email,
                Phone = client.Phone,
                CompanyName = client.CompanyName,
                Notes = client.Notes,
                CreatedAt = client.CreatedAt
            };

            return CreatedAtAction(nameof(GetClient), new { id = client.Id }, readDto);
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
