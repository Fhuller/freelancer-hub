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
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IUserUtils _userUtils;

        public ClientController(IClientService clientService, IUserUtils userUtils)
        {
            _clientService = clientService;
            _userUtils = userUtils;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientReadDto>>> GetClients()
        {
            try
            {
                var userId = _userUtils.GetSupabaseUserId(User);
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
                var userId = _userUtils.GetSupabaseUserId(User);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(Guid id, ClientUpdateDto dto)
        {
            try
            {
                await _clientService.UpdateClientAsync(id, dto);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(Guid id)
        {
            try
            {
                await _clientService.DeleteClientAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
